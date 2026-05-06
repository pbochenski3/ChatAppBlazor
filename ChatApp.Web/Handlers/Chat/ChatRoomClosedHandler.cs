using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatRoomClosedHandler : INotificationHandler<ChatRoomClosedNotification>
    {
        private readonly IChatActionService _chatActionService;
        public ChatRoomClosedHandler(IChatActionService chatActionService) => _chatActionService = chatActionService;

        public async Task Handle(ChatRoomClosedNotification notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleChatCloseAsync();

        }
    }
}
