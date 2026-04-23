using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Message.MarkAsRead
{
    public record MarkMessageAsReadCommand(Guid UserId, Guid ChatId, Guid MessageId) : ICommand<bool>;
}
