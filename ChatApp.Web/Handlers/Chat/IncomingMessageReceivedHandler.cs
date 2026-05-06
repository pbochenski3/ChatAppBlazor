using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class IncomingMessageReceivedHandler : INotificationHandler<IncomingMessageReceivedNotification>
    {
        private readonly IChatActionService _chatActionService;
        public IncomingMessageReceivedHandler(IChatActionService chatActionService) => _chatActionService = chatActionService;

        public async Task Handle(IncomingMessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleIncomingMessageAsync(notification.Message);
        }
    }
}
