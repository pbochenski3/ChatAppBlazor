using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Contact.DeleteContact
{
    public record DeleteContactCommand(Guid PrivateChatId,Guid UserId) : ICommand<bool>;
}
