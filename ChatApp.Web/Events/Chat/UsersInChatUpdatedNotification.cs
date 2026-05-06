using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record UsersInChatUpdatedNotification(Guid ChatId) : INotification;

}
