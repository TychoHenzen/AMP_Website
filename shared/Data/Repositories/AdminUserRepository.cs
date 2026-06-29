#region

using System.Net;
using Amp.Data.Entities;
using Microsoft.Azure.Cosmos;

#endregion

namespace Amp.Data.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly Container _container;

    public AdminUserRepository(CosmosClient cosmosClient, CosmosDbConfig config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseId, config.AdminUsersContainerId);
    }

    public async Task<IEnumerable<AdminUser>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<AdminUser>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'AdminUser' ORDER BY c.username ASC"));

        var results = new List<AdminUser>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<AdminUser?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<AdminUser>(
                id, new PartitionKey("admin"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username)
    {
        var query = _container.GetItemQueryIterator<AdminUser>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'AdminUser' AND c.username = @username")
                .WithParameter("@username", username));

        var results = await query.ReadNextAsync();
        return results.FirstOrDefault();
    }

    public async Task<AdminUser?> GetByEmailAsync(string email)
    {
        var query = _container.GetItemQueryIterator<AdminUser>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'AdminUser' AND c.email = @email")
                .WithParameter("@email", email));

        var results = await query.ReadNextAsync();
        return results.FirstOrDefault();
    }

    public async Task<AdminUser> CreateAsync(AdminUser user)
    {
        var response = await _container.CreateItemAsync(user, new PartitionKey(user.PartitionKey));
        return response.Resource;
    }

    public async Task UpdateAsync(AdminUser user)
    {
        user.LastModifiedAt = DateTime.UtcNow;
        await _container.UpsertItemAsync(user, new PartitionKey(user.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<AdminUser>(id, new PartitionKey("admin"));
    }

    public async Task<bool> UpdatePasswordAsync(string id, string passwordHash)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = passwordHash;
        user.LastModifiedAt = DateTime.UtcNow;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> UpdatePasswordWithTokenAsync(string token, string passwordHash)
    {
        var query = _container.GetItemQueryIterator<AdminUser>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'AdminUser' " +
                    "AND c.passwordResetToken = @token " +
                    "AND c.passwordResetTokenExpiry > @now")
                .WithParameter("@token", token)
                .WithParameter("@now", DateTime.UtcNow));

        var results = await query.ReadNextAsync();
        var user = results.FirstOrDefault();

        if (user == null)
        {
            return false;
        }

        return await UpdatePasswordAsync(user.Id, passwordHash);
    }

    public async Task<bool> SetPasswordResetTokenAsync(string email, string token, DateTime expiry)
    {
        var user = await GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = expiry;
        user.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> IncrementFailedLoginAttemptsAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.FailedLoginAttempts++;
        user.LastModifiedAt = DateTime.UtcNow;

        // Lock account after 5 failed attempts
        if (user.FailedLoginAttempts >= 5)
        {
            user.LockedUntil = DateTime.UtcNow.AddMinutes(30);
        }

        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> ResetFailedLoginAttemptsAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.FailedLoginAttempts = 0;
        user.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> LockUserAsync(string id, DateTime until)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.LockedUntil = until;
        user.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> UnlockUserAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.LockedUntil = null;
        user.FailedLoginAttempts = 0;
        user.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(user);
        return true;
    }

    public async Task UpdateLastLoginAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.LastModifiedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    public async Task<IEnumerable<AdminUser>> GetByRoleAsync(string role)
    {
        var query = _container.GetItemQueryIterator<AdminUser>(
            new QueryDefinition(
                    "SELECT * FROM c WHERE c.type = 'AdminUser' AND c.role = @role ORDER BY c.username ASC")
                .WithParameter("@role", role));

        var results = new List<AdminUser>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<bool> UpdatePermissionsAsync(string id, List<string> permissions)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Permissions = permissions;
        user.LastModifiedAt = DateTime.UtcNow;

        await UpdateAsync(user);
        return true;
    }
}