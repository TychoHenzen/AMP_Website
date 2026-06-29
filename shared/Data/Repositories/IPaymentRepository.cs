#region

using Amp.Data.Entities;

#endregion

namespace Amp.Data.Repositories;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(string id);
    Task<Payment> CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(string id);
    Task<IEnumerable<Payment>> GetByInvoiceIdAsync(string invoiceId);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status);
    Task<IEnumerable<Payment>> GetByCustomerEmailAsync(string email);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> MarkAsCompletedAsync(string id, string? transactionId = null);
    Task<bool> MarkAsFailedAsync(string id);
    Task<bool> ProcessRefundAsync(string id, string reason);
    Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetPaymentMethodTotalsAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, int>> GetPaymentStatusCountsAsync();
    Task<IEnumerable<Payment>> GetRefundsAsync(DateTime startDate, DateTime endDate);
}