using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.User.GetUsersToInvite
{
    public record GetUsersToInviteQuery(Guid UserId, string Query) : IQuery<List<UserSearchResultDTO>>;
}
