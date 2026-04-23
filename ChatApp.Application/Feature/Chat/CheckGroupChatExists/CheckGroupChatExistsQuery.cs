using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Chat.CheckGroupChatExists
{
    public record CheckGroupChatExistsQuery(Guid ChatId, Guid UserId) : IQuery<bool>;
}
