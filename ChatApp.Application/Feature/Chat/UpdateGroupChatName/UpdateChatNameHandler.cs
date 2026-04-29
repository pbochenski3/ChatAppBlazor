using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Chat.UpdateChatName
{
    public class UpdateChatNameHandler : IRequestHandler<UpdateChatNameCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        public UpdateChatNameHandler(IChatRepository chatRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(UpdateChatNameCommand r, CancellationToken cancellationToken)
        {
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive)
            {
                return false;
            }
            var isGroup = await _chatRepo.IsChatGroupAsync(r.ChatId);
                await _chatRepo.UpdateChatNameAsync(r.ChatId, r.Request.NewName);
      
                r.AddEvent(new ChatNameUpdatedNotification(r.ChatId, r.Request));

            return true;
        }
    }
}
