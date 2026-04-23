using ChatApp.Application.DTO;
using MediatR;

namespace ChatApp.Application.Notifications.Message
{
    public record ChatMessageSendedNotification(MessageDTO Message) : INotification;
}
