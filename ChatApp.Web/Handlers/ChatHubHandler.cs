using MediatR;
using static ChatApp.Web.Events.ChatEvents;

namespace ChatApp.Web.Handlers
{
    public class ChatHubHandler : INotificationHandler<RequestToJoinSignalR>
    {
        private readonly ChatHubService _chatHubService;

        public ChatHubHandler(ChatHubService chatHubService)
        {
            _chatHubService = chatHubService;
        }

        public async Task Handle(RequestToJoinSignalR notification, CancellationToken cancellationToken)
        {
            await _chatHubService.JoinChatGroupSignalAsync(notification.ChatId);
        }
    }
}
