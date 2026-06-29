#region

using System.Net;
using DuurzaamDigitaal.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace DuurzaamDigitaal.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly Container _container;

    public PaymentRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.PaymentsContainerId);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'Payment' ORDER BY c.createdAt DESC"));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Payment?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Payment>(
                id, new PartitionKey("payment"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        var response = await _container.CreateItemAsync(payment, new PartitionKey(payment.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(Payment payment)
    {
        payment.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(payment, new PartitionKey(payment.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Payment>(id, new PartitionKey("payment"));
    }

    public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(string invoiceId)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Payment' AND c.invoiceId = @invoiceId")
                .WithParameter("@invoiceId", invoiceId));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Payment' AND c.status = @status ORDER BY c.createdAt DESC")
                .WithParameter("@status", status));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Payment>> GetByCustomerEmailAsync(string email)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Payment' AND c.customerEmail = @email ORDER BY c.createdAt DESC")
                .WithParameter("@email", email));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Payment' " +
                    "AND c.createdAt >= @startDate " +
                    "AND c.createdAt <= @endDate " +
                    "ORDER BY c.createdAt DESC")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<bool> MarkAsCompletedAsync(string id, string? transactionId = null)
    {
        var payment = await GetByIdAsync(id);
        if (payment == null || payment.Status != "Pending")
        {
            return false;
        }

        payment.Status = "Completed";
        payment.CompletedAt = DateTime.UtcNow;
        payment.TransactionId = transactionId;
        payment.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(payment);
        return true;
    }

    public async Task<bool> MarkAsFailedAsync(string id)
    {
        var payment = await GetByIdAsync(id);
        if (payment == null || payment.Status != "Pending")
        {
            return false;
        }

        payment.Status = "Failed";
        payment.FailedAt = DateTime.UtcNow;
        payment.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(payment);
        return true;
    }

    public async Task<bool> ProcessRefundAsync(string id, string reason)
    {
        var payment = await GetByIdAsync(id);
        if (payment == null || payment.Status != "Completed")
        {
            return false;
        }

        payment.Status = "Refunded";
        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundReason = reason;
        payment.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(payment);
        return true;
    }

    public async Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<decimal>(
            new QueryDefinition(
                    "SELECT VALUE SUM(c.amount) FROM c WHERE c.type = 'Payment' " +
                    "AND c.status = 'Completed' " +
                    "AND c.completedAt >= @startDate " +
                    "AND c.completedAt <= @endDate")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var response = await query.ReadNextAsync();
        return response.First();
    }

    public async Task<Dictionary<string, decimal>> GetPaymentMethodTotalsAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<dynamic>(
            new QueryDefinition(
                    "SELECT c.paymentMethod, SUM(c.amount) as total " +
                    "FROM c WHERE c.type = 'Payment' " +
                    "AND c.status = 'Completed' " +
                    "AND c.completedAt >= @startDate " +
                    "AND c.completedAt <= @endDate " +
                    "GROUP BY c.paymentMethod")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var results = new Dictionary<string, decimal>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            foreach (var item in response)
            {
                results[item.paymentMethod.ToString()] = (decimal)item.total;
            }
        }

        return results;
    }

    public async Task<Dictionary<string, int>> GetPaymentStatusCountsAsync()
    {
        var query = _container.GetItemQueryIterator<dynamic>(
            new QueryDefinition(
                "SELECT c.status, COUNT(1) as count " +
                "FROM c WHERE c.type = 'Payment' " +
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

    public async Task<IEnumerable<Payment>> GetRefundsAsync(DateTime startDate, DateTime endDate)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'Payment' " +
                    "AND c.status = 'Refunded' " +
                    "AND c.refundedAt >= @startDate " +
                    "AND c.refundedAt <= @endDate " +
                    "ORDER BY c.refundedAt DESC")
                .WithParameter("@startDate", startDate)
                .WithParameter("@endDate", endDate));

        var results = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }
}