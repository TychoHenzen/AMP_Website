#region

using DuurzaamDigitaal.Data.Entities;

#endregion

namespace DuurzaamDigitaal.Data.Repositories;

public interface IAdminUserRepository
{
    Task<IEnumerable<AdminUser>> GetAllAsync();
    Task<AdminUser?> GetByIdAsync(string id);
    Task<AdminUser?> GetByUsernameAsync(string username);
    Task<AdminUser?> GetByEmailAsync(string email);
    Task<AdminUser> CreateAsync(AdminUser user);
    Task UpdateAsync(AdminUser user);
    Task DeleteAsync(string id);
    Task<bool> UpdatePasswordAsync(string id, string passwordHash);
    Task<bool> UpdatePasswordWithTokenAsync(string token, string passwordHash);
    Task<bool> SetPasswordResetTokenAsync(string email, string token, DateTime expiry);
    Task<bool> IncrementFailedLoginAttemptsAsync(string id);
    Task<bool> ResetFailedLoginAttemptsAsync(string id);
    Task<bool> LockUserAsync(string id, DateTime until);
    Task<bool> UnlockUserAsync(string id);
    Task UpdateLastLoginAsync(string id);
    Task<IEnumerable<AdminUser>> GetByRoleAsync(string role);
    Task<bool> UpdatePermissionsAsync(string id, List<string> permissions);
}