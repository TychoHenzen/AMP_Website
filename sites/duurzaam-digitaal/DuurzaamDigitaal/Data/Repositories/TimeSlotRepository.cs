#region

using System.Net;
using DuurzaamDigitaal.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace DuurzaamDigitaal.Data.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly Container _container;

    public TimeSlotRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.TimeSlotsContainerId);
    }

    public async Task<IEnumerable<TimeSlot>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<TimeSlot>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'TimeSlot' ORDER BY c.startTime ASC"));

        var results = new List<TimeSlot>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<TimeSlot?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<TimeSlot>(
                id, new PartitionKey("timeslot"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
    {
        // Validate the time slot
        if (timeSlot.StartTime >= timeSlot.EndTime)
        {
            throw new ArgumentException("End time must be after start time");
        }

        // Check for overlapping time slots
        var overlapping = await GetOverlappingTimeSlotsAsync(timeSlot.StartTime, timeSlot.EndTime, timeSlot.Location);
        if (overlapping.Any())
        {
            throw new InvalidOperationException("Time slot overlaps with existing slots");
        }

        var response = await _container.CreateItemAsync(timeSlot, new PartitionKey(timeSlot.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(TimeSlot timeSlot)
    {
        timeSlot.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(timeSlot, new PartitionKey(timeSlot.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        var timeSlot = await GetByIdAsync(id);
        if (timeSlot != null && !string.IsNullOrEmpty(timeSlot.AppointmentId))
        {
            throw new InvalidOperationException("Cannot delete time slot with existing appointment");
        }
        
        await _container.DeleteItemAsync<TimeSlot>(id, new PartitionKey("timeslot"));
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(DateTime startDate, DateTime endDate)
    {
        startDate = startDate.Date; // Normalize to start of day
        endDate = endDate.Date.AddDays(1).AddTicks(-1); // Normalize to end of day

        var query = _container.GetItemQueryIterator<TimeSlot>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'TimeSlot' " +
                    "AND c.isAvailable = true " +
                    "AND c.startTime >= @startDate " +
                    "AND c.startTime <= @endDate " +
                    "ORDER BY c.startTime ASC")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var results = new List<TimeSlot>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<bool> ReserveTimeSlotAsync(string id, string appointmentId)
    {
        try
        {
            var timeSlot = await GetByIdAsync(id);
            if (timeSlot == null || !timeSlot.IsAvailable)
            {
                return false;
            }

            // Use optimistic concurrency to prevent double booking
            timeSlot.IsAvailable = false;
            timeSlot.AppointmentId = appointmentId;
            timeSlot.LastModifiedAt = DateTime.UtcNow;

            await _container.UpsertItemAsync(
                timeSlot,
                new PartitionKey(timeSlot.PartitionKey),
                new ItemRequestOptions { IfMatchEtag = timeSlot.ETag });

            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            // Time slot was modified by another request
            return false;
        }
    }

    public async Task ReleaseTimeSlotAsync(string id)
    {
        var timeSlot = await GetByIdAsync(id);
        if (timeSlot != null)
        {
            timeSlot.IsAvailable = true;
            timeSlot.AppointmentId = null;
            timeSlot.LastModifiedAt = DateTime.UtcNow;

            await UpdateAsync(timeSlot);
        }
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByLocationAsync(string location)
    {
        var query = _container.GetItemQueryIterator<TimeSlot>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'TimeSlot' " +
                    "AND c.location = @location " +
                    "ORDER BY c.startTime ASC")
                .WithParameter("@location", location));

        var results = new List<TimeSlot>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    private async Task<IEnumerable<TimeSlot>> GetOverlappingTimeSlotsAsync(DateTime startTime, DateTime endTime, string location)
    {
        var query = _container.GetItemQueryIterator<TimeSlot>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'TimeSlot' " +
                    "AND c.location = @location " +
                    "AND NOT (c.endTime <= @startTime OR c.startTime >= @endTime)")
                .WithParameter("@location", location)
                .WithParameter("@startTime", startTime)
                .WithParameter("@endTime", endTime));

        var results = new List<TimeSlot>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }
}
