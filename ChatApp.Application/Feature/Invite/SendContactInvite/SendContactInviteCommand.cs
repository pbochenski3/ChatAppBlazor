using ChatApp.Application.Common.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Invite.SendContactInvite
{
    public record SendContactInviteCommand(Guid SenderId, Guid ReceiverId) : BaseCommand<bool>;
}
