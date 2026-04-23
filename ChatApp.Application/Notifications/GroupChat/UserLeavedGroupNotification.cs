using ChatApp.Application.DTO;
using MediatR;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record UserLeavedGroupNotification(Guid ChatId, MessageDTO SystemMessage, Guid UserId) : INotification;
}
