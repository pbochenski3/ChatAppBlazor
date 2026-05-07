using ChatApp.Application.Feature.Messages.DeleteMessage;
using ChatApp.Application.Notifications.Message;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Messages.EditMessage
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepo;
        public DeleteMessageHandler(IMessageRepository messageRepo) => _messageRepo = messageRepo;

        public async Task<bool> Handle(DeleteMessageCommand r, CancellationToken cancellationToken)
        {
            var editTime = DateTime.UtcNow;
            var result = await _messageRepo.DeleteMessageAsync(r.MessageId, r.ChatId);
            if (!result)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Nie udało się usunąć wiadomości!"));
                return false;
            }
            r.AddEvent(new MessageDeletedNotification(r.ChatId, r.MessageId));
            return true;

        }
    }
}
