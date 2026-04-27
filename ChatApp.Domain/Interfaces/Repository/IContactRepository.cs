using ChatApp.Domain.Models;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllContactsAsync(Guid userId);
        Task AddContactAsync(Contact contact);
        Task<Contact?> GetContactAsync(Guid contactUserId, Guid userId);
        Task DeleteContactAsync(Guid contactUserId, Guid userId);
        Task<bool> IsContactDeletedAsync(Guid userId, Guid contactUserId);
        Task RestoreContactsAsync(Guid userId, Guid contactUserId);
        Task<bool> IsContactExistingAsync(Guid userId, Guid contactId, CancellationToken token);
    }
}
