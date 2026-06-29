#region

using System.Net;
using Amp.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace Amp.Data.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly Container _container;

    public InvoiceRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.InvoicesContainerId);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'Invoice' ORDER BY c.createdAt DESC"));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Invoice?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Invoice>(
                id, new PartitionKey("invoice"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' AND c.invoiceNumber = @invoiceNumber")
                .WithParameter("@invoiceNumber", invoiceNumber));

        var results = await query.ReadNextAsync();
        return results.FirstOrDefault();
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        var response = await _container.CreateItemAsync(invoice, new PartitionKey(invoice.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        invoice.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(invoice, new PartitionKey(invoice.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Invoice>(id, new PartitionKey("invoice"));
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' AND c.status = @status ORDER BY c.createdAt DESC")
                .WithParameter("@status", status));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerEmailAsync(string email)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' AND c.customerEmail = @email ORDER BY c.createdAt DESC")
                .WithParameter("@email", email));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        var now = DateTime.UtcNow;
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' " +
                    "AND c.status = 'Sent' " +
                    "AND c.dueDate < @now " +
                    "ORDER BY c.dueDate ASC")
                .WithParameter("@now", now));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' " +
                    "AND c.createdAt >= @startDate " +
                    "AND c.createdAt <= @endDate " +
                    "ORDER BY c.createdAt DESC")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<bool> MarkAsPaidAsync(string id, string paymentId)
    {
        var invoice = await GetByIdAsync(id);
        if (invoice == null || invoice.Status == "Paid")
        {
            return false;
        }

        invoice.Status = "Paid";
        invoice.PaidAt = DateTime.UtcNow;
        invoice.PaymentId = paymentId;
        invoice.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(invoice);
        return true;
    }

    public async Task<bool> MarkAsSentAsync(string id)
    {
        var invoice = await GetByIdAsync(id);
        if (invoice == null || invoice.Status != "Draft")
        {
            return false;
        }

        invoice.Status = "Sent";
        invoice.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(invoice);
        return true;
    }

    public async Task<bool> MarkAsCancelledAsync(string id)
    {
        var invoice = await GetByIdAsync(id);
        if (invoice == null || invoice.Status == "Paid")
        {
            return false;
        }

        invoice.Status = "Cancelled";
        invoice.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(invoice);
        return true;
    }

    public async Task<IEnumerable<Invoice>> GetByAppointmentIdAsync(string appointmentId)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' AND c.appointmentId = @appointmentId")
                .WithParameter("@appointmentId", appointmentId));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Invoice>> GetByRefurbishedDeviceIdAsync(string deviceId)
    {
        var query = _container.GetItemQueryIterator<Invoice>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Invoice' AND c.refurbishedDeviceId = @deviceId")
                .WithParameter("@deviceId", deviceId));

        var results = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var query = _container.GetItemQueryIterator<int>(
            new QueryDefinition(
                    "SELECT VALUE COUNT(1) FROM c WHERE c.type = 'Invoice' " +
                    "AND STARTSWITH(c.invoiceNumber, @yearPrefix)")
                .WithParameter("@yearPrefix", $"{year}-"));

        var response = await query.ReadNextAsync();
        var count = response.First() + 1;

        return $"{year}-{count:D5}";
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<decimal>(
            new QueryDefinition(
                    "SELECT VALUE SUM(c.total) FROM c WHERE c.type = 'Invoice' " +
                    "AND c.status = 'Paid' " +
                    "AND c.paidAt >= @startDate " +
                    "AND c.paidAt <= @endDate")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var response = await query.ReadNextAsync();
        return response.First();
    }

    public async Task<Dictionary<string, int>> GetInvoiceStatusCountsAsync()
    {
        var query = _container.GetItemQueryIterator<dynamic>(
            new QueryDefinition(
                "SELECT c.status, COUNT(1) as count " +
                "FROM c WHERE c.type = 'Invoice' " +
                "GROUP BY c.status"));

        var results = new Dictionary<string, int>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            foreach (var item in response)
            {
                results[item.status.ToString()] = (int)item.count;
            }
        }

        return results;
    }
}