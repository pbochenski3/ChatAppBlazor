using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Contacts.GetUserContacts
{
    public record GetUserContactsQuery(Guid UserId, Guid ChatId) : IQuery<List<ContactDTO>>;
}
