using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Contact.GetUserContacts
{
    public record GetUserContactsQuery(Guid UserId, Guid ChatId) : IQuery<List<ContactDTO>>;
}
