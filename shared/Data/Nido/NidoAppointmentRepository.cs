using System.Net;
using Microsoft.Azure.Cosmos;

namespace Amp.Data.Nido;

public class NidoAppointmentRepository : INidoAppointmentRepository
{
    private readonly Container _container;

    public NidoAppointmentRepository(CosmosClient cosmosClient, NidoConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.AppointmentsContainerId);
    }

    /// <summary>
    /// Reserves a slot atomically. The doc id is deterministic (date_time) so two concurrent
    /// bookings for the same hour can't both win — Cosmos rejects the second with 409, which we
    /// surface as <see cref="SlotUnavailableException"/>. No read-then-write race.
    /// </summary>
    public async Task<NidoAppointment> CreateAsync(NidoAppointment appointment)
    {
        appointment.PartitionKey = appointment.Date;
        appointment.Id = SlotId(appointment.Date, appointment.Time);
        try
        {
            var response = await _container.CreateItemAsync(appointment, new PartitionKey(appointment.Date));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            throw new SlotUnavailableException(appointment.Date, appointment.Time);
        }
    }

    public async Task<IReadOnlyList<NidoAppointment>> GetByDateAsync(string date)
    {
        var query = _container.GetItemQueryIterator<NidoAppointment>(
            new QueryDefinition("SELECT * FROM c WHERE c.date = @date AND c.status != 'cancelled'")
                .WithParameter("@date", date),
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(date) });

        var results = new List<NidoAppointment>();
        while (query.HasMoreResults)
            results.AddRange(await query.ReadNextAsync());
        return results;
    }

    public async Task<IReadOnlyList<NidoAppointment>> GetUpcomingAsync(string fromDate)
    {
        // Cross-partition; sort in memory to avoid requiring a composite index.
        var query = _container.GetItemQueryIterator<NidoAppointment>(
            new QueryDefinition("SELECT * FROM c WHERE c.date >= @from AND c.status != 'cancelled'")
                .WithParameter("@from", fromDate));

        var results = new List<NidoAppointment>();
        while (query.HasMoreResults)
            results.AddRange(await query.ReadNextAsync());

        return results
            .OrderBy(a => a.Date, StringComparer.Ordinal)
            .ThenBy(a => a.Time, StringComparer.Ordinal)
            .ToList();
    }

    public async Task DeleteAsync(string id, string date)
    {
        try
        {
            await _container.DeleteItemAsync<NidoAppointment>(id, new PartitionKey(date));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // already gone — treat delete as idempotent
        }
    }

    private static string SlotId(string date, string time) => $"{date}_{time}";
}
