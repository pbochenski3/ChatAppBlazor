using MediatR;

namespace ChatApp.Web.Events.SignalR
{
    public record RequestToJoinSignalRNotification(Guid ChatId) : INotification;
}
