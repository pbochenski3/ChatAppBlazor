using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllContactsAsync(Guid userId);
        Task AddContactAsync(Contact contact);
        Task<Contact?> GetContactAsync(Guid contactUserId, Guid userId);
        Task DeleteContactAsync(Guid contactUserId, Guid userId);
        Task<bool> TryRestoreContactsAsync(Guid userId, Guid contactUserId);
    }
}
