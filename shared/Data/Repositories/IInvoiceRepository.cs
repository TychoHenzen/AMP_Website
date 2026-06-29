#region

using Amp.Data.Entities;

#endregion

namespace Amp.Data.Repositories;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(string id);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
    Task DeleteAsync(string id);
    Task<IEnumerable<Invoice>> GetByStatusAsync(string status);
    Task<IEnumerable<Invoice>> GetByCustomerEmailAsync(string email);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> MarkAsPaidAsync(string id, string paymentId);
    Task<bool> MarkAsSentAsync(string id);
    Task<bool> MarkAsCancelledAsync(string id);
    Task<IEnumerable<Invoice>> GetByAppointmentIdAsync(string appointmentId);
    Task<IEnumerable<Invoice>> GetByRefurbishedDeviceIdAsync(string deviceId);
    Task<string> GenerateInvoiceNumberAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, int>> GetInvoiceStatusCountsAsync();
}