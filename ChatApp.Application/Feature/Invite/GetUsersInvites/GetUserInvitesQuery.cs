using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Invite.GetUsersInvites
{
    public record GetUserInvitesQuery(Guid UserId) : IQuery<List<InviteDTO>>;

}
