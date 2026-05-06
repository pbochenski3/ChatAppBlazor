using ChatApp.Application.DTO;
using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatUpdatedHandler : INotificationHandler<ChatUpdatedNotification>
    {
        private readonly IChatActionService _chatActionService;
        public ChatUpdatedHandler(IChatActionService chatActionService) => _chatActionService = chatActionService;
        public async Task Handle(ChatUpdatedNotification notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleChatLoadAsync(new ContactSelectedArgs(notification.ChatId, notification.Force));
        }
    }
}
