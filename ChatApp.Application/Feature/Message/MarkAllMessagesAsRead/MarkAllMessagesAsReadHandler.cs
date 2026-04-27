using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Message.MarkAllMessagesAsRead
{
    public class MarkAllMessagesAsReadHandler : IRequestHandler<MarkAllMessagesAsReadCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        public MarkAllMessagesAsReadHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(MarkAllMessagesAsReadCommand r, CancellationToken cancellationToken)
        {
            var userChat = await _userChatRepo.GetUserChatAsync(r.ChatId, r.UserId, cancellationToken);
            var lastMessageId = userChat?.LastMessageID;
            if (lastMessageId.HasValue)
            {
                await _userChatRepo.UpdateLastReadMessageAsync(r.UserId, r.ChatId, lastMessageId.Value);
                return true;
            }
            return false;
        }
    }
}
