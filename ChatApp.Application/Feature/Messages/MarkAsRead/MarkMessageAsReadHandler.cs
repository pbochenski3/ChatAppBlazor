using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Messages.MarkAsRead
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
            await _userChatRepo.UpdateLastReadMessageAsync(new HashSet<Guid> { r.UserId }, r.ChatId, r.MessageId);

            return true;
        }
    }
}
