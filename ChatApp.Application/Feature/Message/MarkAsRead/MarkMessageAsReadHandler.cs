using ChatApp.Domain.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Message.MarkAsRead
{
    public class MarkAllAsReadHandler : IRequestHandler<MarkMessageAsReadCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;

        public MarkAllAsReadHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }

        public async Task<bool> Handle(MarkMessageAsReadCommand r, CancellationToken token)
        {
            await _userChatRepo.UpdateLastReadMessageAsync(r.UserId, r.ChatId, r.MessageId);

            return true;
        }
    }
}
