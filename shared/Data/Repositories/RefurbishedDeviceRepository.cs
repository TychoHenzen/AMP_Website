#region

using System.Net;
using Amp.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace Amp.Data.Repositories;

public class RefurbishedDeviceRepository : IRefurbishedDeviceRepository
{
    private readonly Container _container;

    public RefurbishedDeviceRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.RefurbishedDevicesContainerId);
    }

    public async Task<IEnumerable<RefurbishedDevice>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<RefurbishedDevice>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'RefurbishedDevice' ORDER BY c.createdAt DESC"));

        var results = new List<RefurbishedDevice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<RefurbishedDevice?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<RefurbishedDevice>(
                id, new PartitionKey("device"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<RefurbishedDevice> CreateAsync(RefurbishedDevice device)
    {
        var response = await _container.CreateItemAsync(device, new PartitionKey(device.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(RefurbishedDevice device)
    {
        device.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(device, new PartitionKey(device.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<RefurbishedDevice>(id, new PartitionKey("device"));
    }

    public async Task<IEnumerable<RefurbishedDevice>> GetByStatusAsync(string status)
    {
        var query = _container.GetItemQueryIterator<RefurbishedDevice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'RefurbishedDevice' AND c.status = @status ORDER BY c.createdAt DESC")
                .WithParameter("@status", status));

        var results = new List<RefurbishedDevice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<RefurbishedDevice>> GetByDeviceTypeAsync(string deviceType)
    {
        var query = _container.GetItemQueryIterator<RefurbishedDevice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'RefurbishedDevice' AND c.deviceType = @deviceType ORDER BY c.price ASC")
                .WithParameter("@deviceType", deviceType));

        var results = new List<RefurbishedDevice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<RefurbishedDevice>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var query = _container.GetItemQueryIterator<RefurbishedDevice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'RefurbishedDevice' " +
                    "AND c.price >= @minPrice AND c.price <= @maxPrice " +
                    "ORDER BY c.price ASC")
                .WithParameter("@minPrice", minPrice)
                .WithParameter("@maxPrice", maxPrice));

        var results = new List<RefurbishedDevice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<bool> ReserveDeviceAsync(string id, DateTime reservedUntil)
    {
        var device = await GetByIdAsync(id);
        if (device == null || device.Status != "Available")
        {
            return false;
        }

        device.Status = "Reserved";
        device.ReservedUntil = reservedUntil;
        device.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(device);
        return true;
    }

    public async Task ReleaseReservationAsync(string id)
    {
        var device = await GetByIdAsync(id);
        if (device != null && device.Status == "Reserved")
        {
            device.Status = "Available";
            device.ReservedUntil = null;
            device.LastModifiedAt = DateTime.UtcNow;

            await UpdateAsync(device);
        }
    }

    public async Task MarkAsSoldAsync(string id)
    {
        var device = await GetByIdAsync(id);
        if (device != null && device.Status != "Sold")
        {
            device.Status = "Sold";
            device.SoldAt = DateTime.UtcNow;
            device.LastModifiedAt = DateTime.UtcNow;

            await UpdateAsync(device);
        }
    }

    public async Task<IEnumerable<RefurbishedDevice>> SearchAsync(string searchTerm)
    {
        var query = _container.GetItemQueryIterator<RefurbishedDevice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'RefurbishedDevice' " +
                    "AND (CONTAINS(LOWER(c.name), LOWER(@search)) " +
                    "OR CONTAINS(LOWER(c.brand), LOWER(@search)) " +
                    "OR CONTAINS(LOWER(c.model), LOWER(@search)) " +
                    "OR CONTAINS(LOWER(c.deviceType), LOWER(@search)) " +
                    "OR CONTAINS(LOWER(c.description), LOWER(@search))) " +
                    "ORDER BY c.createdAt DESC")
                .WithParameter("@search", searchTerm.ToLower()));

        var results = new List<RefurbishedDevice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }
}