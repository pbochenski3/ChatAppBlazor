using MediatR;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record GroupChatCreatedNotification(Guid chatId, HashSet<Guid> usersToNofity) : INotification;
}
