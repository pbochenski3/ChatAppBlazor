using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Messages.MarkAllMessagesAsRead
{
    public record MarkAllMessagesAsReadCommand(Guid UserId, Guid ChatId) : ICommand<bool>;
}
