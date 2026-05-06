using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class RequestToUpdateFlagOnChatHandler : INotificationHandler<RequestToUpdateFlagOnChatNotification>
    {
        private readonly IChatActionService _chatActionService;
        public RequestToUpdateFlagOnChatHandler(IChatActionService chatActionService) => _chatActionService = chatActionService;

        public async Task Handle(RequestToUpdateFlagOnChatNotification n, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleUpdateFlagOnChatAsync(n.UserId, n.ChatId, n.Flag);

        }
    }
}
