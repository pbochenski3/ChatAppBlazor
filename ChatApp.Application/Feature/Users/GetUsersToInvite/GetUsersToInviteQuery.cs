using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Result;

namespace ChatApp.Application.Feature.Users.GetUsersToInvite
{
    public record GetUsersToInviteQuery(Guid UserId, string Query) : IQuery<List<UserSearchResultDTO>>;
}
