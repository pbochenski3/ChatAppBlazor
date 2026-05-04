using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Chats.GetChatUsers
{
    public record GetChatUsersIdsQuery(Guid ChatId) : IQuery<HashSet<Guid>>;
}
