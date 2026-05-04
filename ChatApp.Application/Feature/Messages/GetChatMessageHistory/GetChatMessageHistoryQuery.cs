using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Messages.GetChatMessageHistory
{
    public record GetChatMessageHistoryQuery(Guid UserId, Guid ChatId) : IQuery<List<MessageDTO>>;
}
