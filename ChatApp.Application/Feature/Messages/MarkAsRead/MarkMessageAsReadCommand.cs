using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Messages.MarkAsRead
{
    public record MarkMessageAsReadCommand(Guid UserId, Guid ChatId, Guid MessageId) : ICommand<bool>;
}
