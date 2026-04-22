using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.LeaveGroupChatAsync
{
    public class LeaveGroupChatHandler : IRequestHandler<LeaveGroupChatCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMediator _mediator;
        public LeaveGroupChatHandler(IChatRepository chatRepo, IMessageRepository messageRepo, IMediator mediator, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _messageRepo = messageRepo;
            _mediator = mediator;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(LeaveGroupChatCommand r, CancellationToken cancellationToken)
        {
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
                Content = $"{r.Username} opuścił czat.",
                SenderUsername = "SYSTEM",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };
            var message = new Domain.Models.Message
            {
                Content = $"{r.Username} opuścił czat.",
                ChatID = r.ChatId,
                MessageID = Guid.CreateVersion7(),
                SentAt = DateTime.UtcNow,
                MessageType = MessageType.System,

            };

            await _messageRepo.AddMessageAsync(message);
            await _mediator.Publish(new UserLeavedGroupNotification(r.ChatId, systemMessage, r.UserId));
            return true;
        }
    }
}
