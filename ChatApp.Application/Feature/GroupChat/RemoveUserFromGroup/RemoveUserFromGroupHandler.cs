using ChatApp.Application.DTO;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.RemoveUserFromGroup
{
    public class RemoveUserFromGroupHandler : IRequestHandler<RemoveUserFromGroupCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMessageRepository _messageRepo;
        public RemoveUserFromGroupHandler(IChatRepository chatRepo, IMessageRepository messageRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(RemoveUserFromGroupCommand r, CancellationToken cancellationToken)
        {
            var isAdmin = await _userChatRepo.GetUserAdminFlagAsync(r.AdminId, r.ChatId);
            if (!isAdmin)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Nie posiadasz uprawnień!"));
                return false;
            }
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive)
            {
                return false;
            }
            await _userChatRepo.ArchiveChatAsync(r.ChatId, r.UserId);

            var systemMessage = Message.CreateSystemMessage(r.ChatId, $"{r.AdminName} usunął użytkownika {r.RemovedUserAlias} z grupy!.");
            await _messageRepo.AddMessageAsync(systemMessage);
            r.AddEvent(new UserLeavedGroupNotification(r.ChatId, systemMessage.Adapt<MessageDTO>(), r.UserId, true));
            return true;
        }
    }
}
