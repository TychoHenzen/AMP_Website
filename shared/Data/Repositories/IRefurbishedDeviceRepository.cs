#region

using Amp.Data.Entities;

#endregion

namespace Amp.Data.Repositories;

public interface IRefurbishedDeviceRepository
{
    Task<IEnumerable<RefurbishedDevice>> GetAllAsync();
    Task<RefurbishedDevice?> GetByIdAsync(string id);
    Task<RefurbishedDevice> CreateAsync(RefurbishedDevice device);
    Task UpdateAsync(RefurbishedDevice device);
    Task DeleteAsync(string id);
    Task<IEnumerable<RefurbishedDevice>> GetByStatusAsync(string status);
    Task<IEnumerable<RefurbishedDevice>> GetByDeviceTypeAsync(string deviceType);
    Task<IEnumerable<RefurbishedDevice>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<bool> ReserveDeviceAsync(string id, DateTime reservedUntil);
    Task ReleaseReservationAsync(string id);
    Task MarkAsSoldAsync(string id);
    Task<IEnumerable<RefurbishedDevice>> SearchAsync(string searchTerm);
}