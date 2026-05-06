using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.DeleteChat
{
    public class DeleteChatHandler : IRequestHandler<DeleteChatCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        public DeleteChatHandler(IChatRepository chatRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(DeleteChatCommand r, CancellationToken cancellationToken)
        {
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive == true)
            {
                try
                {
                    await _userChatRepo.MarkChatAsDeletedAsync(r.ChatId, r.UserId);
                    //await _chatRepo.TryDeleteChatIfEmptyAsync(r.ChatId);
                }
                catch (Exception)
                {
                    return false;
                }
                r.AddEvent(new ChatDeletedNotification(r.ChatId, r.UserId));
                return true;
            }
            return false;
        }
    }
}
