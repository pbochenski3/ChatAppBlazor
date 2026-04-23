using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Message.MarkAllMessagesAsRead
{
    public record MarkAllMessagesAsReadCommand(Guid UserId, Guid ChatId) : ICommand<bool>;
}
