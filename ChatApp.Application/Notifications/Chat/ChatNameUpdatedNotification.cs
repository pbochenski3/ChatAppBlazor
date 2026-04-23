using ChatApp.Application.DTO.Requests;
using MediatR;

namespace ChatApp.Application.Notifications.Chat
{
    public record ChatNameUpdatedNotification(Guid ChatId, ChangeChatNameRequest Request) : INotification;
}
