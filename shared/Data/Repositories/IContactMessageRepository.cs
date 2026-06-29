#region

using Amp.Data.Entities;

#endregion

namespace Amp.Data.Repositories;

public interface IContactMessageRepository
{
    Task<IEnumerable<ContactMessage>> GetAllAsync();
    Task<ContactMessage?> GetByIdAsync(string id);
    Task<ContactMessage> CreateAsync(ContactMessage message);
    Task UpdateAsync(ContactMessage message);
    Task DeleteAsync(string id);
    Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync();
    Task MarkAsReadAsync(string id);
}