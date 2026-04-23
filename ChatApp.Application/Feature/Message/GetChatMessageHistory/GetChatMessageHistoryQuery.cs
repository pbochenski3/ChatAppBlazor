using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Message.GetChatMessageHistory
{
    public record GetChatMessageHistoryQuery(Guid UserId, Guid ChatId) : IQuery<List<MessageDTO>>;
}
