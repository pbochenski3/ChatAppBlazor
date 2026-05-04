using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Chats.CheckGroupChatExists
{
    public record CheckGroupChatExistsQuery(Guid ChatId, Guid UserId) : IQuery<bool>;
}
