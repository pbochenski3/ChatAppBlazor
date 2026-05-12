using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Message;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class ChatMessageSendedHandler : INotificationHandler<ChatMessageSendedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        private readonly IUserChatRepository _userChatRepo;


        public ChatMessageSendedHandler(IUserChatRepository userChatRepo, ISignalRNotificationService signalR)
        {
            _signalR = signalR;
            _userChatRepo = userChatRepo;
        }
        public async Task Handle(ChatMessageSendedNotification n, CancellationToken cancellationToken)
        {
            var _message = n.Message;
            await _signalR.SendToGroup(_message.ChatID.ToString(), "ReceiveMessage", _message);
            var chat = await _userChatRepo.GetUsersInChatIdAsync(_message.ChatID);
            var participants = chat.Select(id => id.ToString()).ToList();
            await _signalR.SendToUsers(participants, "UpdateLastMessage", _message);
        }
    }
}
