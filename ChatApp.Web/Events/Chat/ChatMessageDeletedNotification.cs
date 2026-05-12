using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record ChatMessegeDeletedNotification(Guid MessageId, Guid ChatId, Guid userId) : INotification;
}
