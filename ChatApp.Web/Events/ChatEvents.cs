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
    }
}
