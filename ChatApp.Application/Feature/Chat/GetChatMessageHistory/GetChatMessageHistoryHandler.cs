using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatMessageHistory
{
    public class GetChatHistoryHandler : IRequestHandler<GetChatMessageHistoryQuery, List<MessageDTO>>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        public GetChatHistoryHandler(IMessageRepository messageRepo,IUserChatRepository userChatRepo)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task<List<MessageDTO>> Handle(GetChatMessageHistoryQuery r, CancellationToken cancellationToken)
        {
            bool isArchive = await _userChatRepo.IsChatArchivedAsync(r.ChatId, r.UserId);
            DateTime? cutoffDate = null;

            if (isArchive)
            {
                cutoffDate = await _userChatRepo.GetLastMessageDateAsync(r.UserId, r.ChatId);
            }

            var messages = await _messageRepo.GetMessageHistoryAsync(r.UserId, r.ChatId, cutoffDate, cancellationToken);

            if (messages == null || !messages.Any())
            {
                return new List<MessageDTO>();
            }

            return messages.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                imageUrl = m.imageUrl,
                SentAt = m.SentAt,
                SenderID = m.SenderID,
                SenderUsername = m.MessageType == MessageType.System ? "SYSTEM" : (m.Sender?.Username ?? "Unknown"),
                ChatID = m.ChatID,
                MessageType = m.MessageType,
            }).ToList();
        }
    }
}
