using ChatApp.Application.DTO;
using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record IncomingMessageReceivedNotification(MessageDTO Message) : INotification;
}
