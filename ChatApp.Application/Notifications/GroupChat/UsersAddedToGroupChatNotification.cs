using ChatApp.Application.DTO;
using MediatR;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record UsersAddedToGroupChatNotification(Guid GroupChatId, MessageDTO SystemMessage, HashSet<Guid> UsersInChat) : INotification;


}
