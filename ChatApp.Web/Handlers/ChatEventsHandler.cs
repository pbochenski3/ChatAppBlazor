using ChatApp.Application.DTO;
using ChatApp.Web.Events;
using ChatApp.Web.Services.Actions.Interfaces;
using MediatR;
using static ChatApp.Web.Events.ChatEvents;

namespace ChatApp.Web.Handlers
{
    public class ChatEventsHandler :
        INotificationHandler<IncomingMessageReceived>,
        INotificationHandler<ChatRoomClosed>,
        INotificationHandler<ChatUpdated>,
        INotificationHandler<UsersInChatUpdated>
    {
        private readonly IChatActionService _chatActionService;
        public ChatEventsHandler(IChatActionService chatActionService)
        {
            _chatActionService = chatActionService;
        }
        public async Task Handle(ChatEvents.IncomingMessageReceived notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleIncomingMessageAsync(notification.Message);
        }

        public async Task Handle(ChatEvents.ChatRoomClosed notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleChatCloseAsync();
        }

        public async Task Handle(ChatEvents.ChatUpdated notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleChatLoadAsync(new ContactSelectedArgs(notification.ChatId, notification.Force));
        }

        public async Task Handle(ChatEvents.UsersInChatUpdated notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleUserOnGroupLoadAsync(notification.ChatId);
        }
    }
}
