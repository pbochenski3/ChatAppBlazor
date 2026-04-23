using MediatR;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record UsersAddedToGroupChatNotification(Guid GroupChatId, Domain.Models.Message SystemMessage, HashSet<Guid> UsersInChat) : INotification;


}
