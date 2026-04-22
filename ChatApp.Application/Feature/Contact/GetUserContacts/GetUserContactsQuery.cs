using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Contact.GetUserContacts
{
    public record GetUserContactsQuery(Guid UserId,Guid ChatId) : IQuery<List<ContactDTO>>;
}
