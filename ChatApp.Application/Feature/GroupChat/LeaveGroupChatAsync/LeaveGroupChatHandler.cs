using ChatApp.Application.DTO;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.LeaveGroupChatAsync
{
    public class LeaveGroupChatHandler : IRequestHandler<LeaveGroupChatCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMessageRepository _messageRepo;
        public LeaveGroupChatHandler(IChatRepository chatRepo, IMessageRepository messageRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(LeaveGroupChatCommand r, CancellationToken cancellationToken)
        {
            var groupHaveAdmin = await _userChatRepo.CheckIfGroupHaveAdminAsync(r.ChatId, r.UserId);
            if (!groupHaveAdmin)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Zanim opuścisz grupe nadaj komuś admina!"));
                return false;
            }
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive)
            {
                return false;
            }
            await _userChatRepo.ArchiveChatAsync(r.ChatId, r.UserId);

            var systemMessage = Message.CreateSystemMessage(r.ChatId, $"{r.Username} opuścił czat.");

            await _messageRepo.AddMessageAsync(systemMessage);
            r.AddEvent(new UserLeavedGroupNotification(r.ChatId, systemMessage.Adapt<MessageDTO>(), r.UserId, false));
            return true;
        }
    }
}
