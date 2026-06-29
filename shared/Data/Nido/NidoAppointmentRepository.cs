using Microsoft.Azure.Cosmos;

namespace Amp.Data.Nido;

public class NidoAppointmentRepository : INidoAppointmentRepository
{
    private readonly Container _container;

    public NidoAppointmentRepository(CosmosClient cosmosClient, NidoConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.AppointmentsContainerId);
    }

    public async Task<NidoAppointment> CreateAsync(NidoAppointment appointment)
    {
        appointment.PartitionKey = appointment.Date;
        var response = await _container.CreateItemAsync(appointment, new PartitionKey(appointment.Date));
        return response.Resource;
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
}
