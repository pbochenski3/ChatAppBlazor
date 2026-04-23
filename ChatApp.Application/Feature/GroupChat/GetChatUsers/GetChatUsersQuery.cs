using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.GroupChat.GetChatUsers
{
    public record GetChatUsersQuery(Guid ChatId) : IQuery<HashSet<UserDTO>>;
}
