using ChatApp.Application.DTO;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
            if (isArchive == true)
            {
                return false;
            }
            await _userChatRepo.ArchiveChatAsync(r.ChatId, r.UserId);


            var systemMessage = new MessageDTO
            {
                ChatID = r.ChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{r.AdminName} usunął użytkownika {r.RemovedUserAlias} z grupy!.",
                SenderUsername = "SYSTEM",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };
            var message = systemMessage.Adapt<Domain.Models.Message>();


            await _messageRepo.AddMessageAsync(message);
            r.AddEvent(new UserLeavedGroupNotification(r.ChatId, systemMessage, r.UserId));
            return true;
        }
    }
}
