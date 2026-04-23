using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Chats;

namespace ChatApp.Application.Feature.Chat.GetChatDetails
{
    public record GetChatDetailsQuery(Guid ChatId, Guid UserId) : IQuery<UserChatDTO>;
}
