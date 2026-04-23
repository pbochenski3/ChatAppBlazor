using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Chat.GetChatUsers
{
    public record GetChatUsersIdsQuery(Guid ChatId) : IQuery<HashSet<Guid>>;
}
