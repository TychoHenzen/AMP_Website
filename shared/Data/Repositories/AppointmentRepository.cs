#region

using System.Net;
using Amp.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace Amp.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly Container _container;

    public AppointmentRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.AppointmentsContainerId);
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
    {
        var query = _container.GetItemQueryIterator<Appointment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Appointment' AND c.status = @status ORDER BY c.createdAt DESC")
                .WithParameter("@status", status));

        var results = new List<Appointment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Appointment>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'Appointment' ORDER BY c.createdAt DESC"));

        var results = new List<Appointment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Appointment>(
                id, new PartitionKey("appointment"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        var response = await _container.CreateItemAsync(appointment, new PartitionKey(appointment.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        appointment.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(appointment, new PartitionKey(appointment.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Appointment>(id, new PartitionKey("appointment"));
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync()
    {
        var now = DateTime.UtcNow;
        var query = _container.GetItemQueryIterator<Appointment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Appointment' " +
                    "AND c.status = 'Scheduled' " +
                    "AND c.timeSlot.startTime > @now " +
                    "ORDER BY c.timeSlot.startTime ASC")
                .WithParameter("@now", now));

        var results = new List<Appointment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task UpdateStatusAsync(string id, string status)
    {
        var appointment = await GetByIdAsync(id);
        if (appointment != null)
        {
            appointment.Status = status;
            appointment.LastModifiedAt = DateTime.UtcNow;

            switch (status)
            {
                case "Completed":
                    appointment.CompletedAt = DateTime.UtcNow;
                    break;
                case "Cancelled":
                    appointment.CancelledAt = DateTime.UtcNow;
                    break;
            }

            await UpdateAsync(appointment);
        }
    }
}