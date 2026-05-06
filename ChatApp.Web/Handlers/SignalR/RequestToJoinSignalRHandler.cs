using ChatApp.Web.Events.SignalR;
using MediatR;

namespace ChatApp.Web.Handlers
{
    public class RequestToJoinSignalRHandler : INotificationHandler<RequestToJoinSignalRNotification>
    {
        private readonly ChatHubService _chatHubService;

        public RequestToJoinSignalRHandler(ChatHubService chatHubService) => _chatHubService = chatHubService;


        public async Task Handle(RequestToJoinSignalRNotification notification, CancellationToken cancellationToken)
        {
            await _chatHubService.JoinChatGroupSignalAsync(notification.ChatId);
        }
    }
}
