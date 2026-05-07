using ChatApp.Application.Notifications.Message;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Messages.EditMessage
{
    public class EditMessageHandler : IRequestHandler<EditMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepo;
        public EditMessageHandler(IMessageRepository messageRepo) => _messageRepo = messageRepo;

        public async Task<bool> Handle(EditMessageCommand r, CancellationToken cancellationToken)
        {
            var editTime = DateTime.UtcNow;
            var result = await _messageRepo.UpdateMessageContentAsync(r.MessageId, r.ChatId, r.Content, editTime);
            if (!result)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Nie udało się edytować wiadomości!"));
                return false;
            }
            r.AddEvent(new MessageEditedNotification(r.ChatId, r.MessageId, r.Content, editTime));
            return true;

        }
    }
}
