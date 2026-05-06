using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record ChatRoomClosedNotification() : INotification;
}
