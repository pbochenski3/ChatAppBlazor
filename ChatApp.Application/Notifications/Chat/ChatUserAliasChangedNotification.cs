using ChatApp.Application.DTO.Requests;
using MediatR;

namespace ChatApp.Application.Notifications.Chat
{
    public record ChatUserAliasChangedNotification(Guid ChatId, ChangeAliasRequest Request) : INotification;
}
