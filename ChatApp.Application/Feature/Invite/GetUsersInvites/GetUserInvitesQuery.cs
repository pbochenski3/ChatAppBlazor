using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Invite.GetUsersInvites
{
    public record GetUserInvitesQuery(Guid UserId) : IQuery<List<InviteDTO>>;

}
