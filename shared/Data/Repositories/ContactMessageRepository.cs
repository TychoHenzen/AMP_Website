#region

using System.Net;
using Amp.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace Amp.Data.Repositories;

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly Container _container;

    public ContactMessageRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.MessagesContainerId);
    }

    public async Task<IEnumerable<ContactMessage>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<ContactMessage>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'ContactMessage' ORDER BY c.createdAt DESC"));

        var results = new List<ContactMessage>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<ContactMessage?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<ContactMessage>(
                id, new PartitionKey("message"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<ContactMessage> CreateAsync(ContactMessage message)
    {
        var response = await _container.CreateItemAsync(message, new PartitionKey(message.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(ContactMessage message)
    {
        message.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(message, new PartitionKey(message.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<ContactMessage>(id, new PartitionKey("message"));
    }

    public async Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync()
    {
        var query = _container.GetItemQueryIterator<ContactMessage>(
            new QueryDefinition(
                "SELECT * FROM c WHERE c.type = 'ContactMessage' AND c.isRead = false ORDER BY c.createdAt DESC"));

        var results = new List<ContactMessage>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task MarkAsReadAsync(string id)
    {
        var message = await GetByIdAsync(id);
        if (message != null)
        {
            message.IsRead = true;
            message.LastModifiedAt = DateTime.UtcNow;
            await UpdateAsync(message);
        }
    }
}