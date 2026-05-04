using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Invites.GetUsersInvites
{
    public record GetUserInvitesQuery(Guid UserId) : IQuery<List<InviteDTO>>;

}
