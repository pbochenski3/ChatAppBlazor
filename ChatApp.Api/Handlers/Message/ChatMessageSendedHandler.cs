using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Notifications.Message;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Message
{
    public class ChatMessageSendedHandler : INotificationHandler<ChatMessageSendedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;


        public ChatMessageSendedHandler(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _chatService = chatService;
        }
        public async Task Handle(ChatMessageSendedNotification n, CancellationToken cancellationToken)
        {
            var _message = n.Message;
            await _hubContext.Clients.Group(_message.ChatID.ToString()).SendAsync("ReceiveMessage", _message);
            var chat = await _chatService.GetUsersInChatIdAsync(_message.ChatID);
            var participants = chat.Select(id => id.ToString()).ToList();
            await _hubContext.Clients.Users(participants).SendAsync("UpdateLastMessage", _message.ChatID, _message.SenderUsername, _message.Content);
        }
    }
}
