using ChatApp.Application.DTO;
using ChatApp.Application.Events;
using MediatR;

namespace ChatApp.Web.Events
{
    public class ChatEvents
    {
        public record IncomingMessageReceived(MessageDTO Message) : INotification;
        public record ChatRoomClosed() : INotification;
        public record ChatUpdated(Guid ChatId, bool Force) : INotification;
        public record UsersInChatUpdated(Guid ChatId) : INotification;
        public record RequestToJoinSignalR(Guid chatId) : INotification;
        public record ChatDeleted() : INotification;
        public record ChatDeletionFailed() : INotification;
        public record ChatLeft() : INotification;
        public record ChatLeavingFailed() : INotification;
        public record ContactDeleted() : INotification;
        public record ContactDeletionFailed() : INotification;
        public record LoadingFailed() : INotification;
        public record ChatNameChange(string chatName) : INotification;
        public record ChangingChatNameFailed() : INotification;
        public record AddingUsersToGroupFailed() : INotification;
    }
}
