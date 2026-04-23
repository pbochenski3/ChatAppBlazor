using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Invite.HandleInviteAction
{
    public record HandleInviteActionCommand(InviteActionRequest Request) : ICommand<bool>;
}
