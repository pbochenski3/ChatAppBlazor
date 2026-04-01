using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatService _userChatService;
        private readonly IChatReadStatusService _readStatusService;

        public MessageService(IMessageRepository messageRepo,
            IUserChatService userChatService,
            IChatReadStatusService readStatusService)
        {
            _messageRepo = messageRepo;
            _userChatService = userChatService;
            _readStatusService = readStatusService;
        }

        public async Task SaveMessageAsync(MessageDTO messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Content))
            {
                throw new Exception("Message content cannot be empty");
            }

            var message = new Message
            {
                Content = messageDto.Content,
                SenderID = messageDto.SenderID,
                ChatID = messageDto.ChatID,
                MessageID = messageDto.MessageID,
                SentAt = messageDto.SentAt,
                IsSystemMessage = messageDto.IsSystemMessage,
            };
            await _messageRepo.AddMessageAsync(message);
        }

        public async Task<List<MessageDTO>> GetChatHistoryAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            bool isArchive = await _userChatService.IsChatArchivedAsync(chatId, userId);
            DateTime? cutoffDate = null;

            if (isArchive)
            {
                cutoffDate = await _readStatusService.GetLastSeenMessageAtAsync(userId, chatId);
            }

            var messages = await _messageRepo.GetMessageHistoryAsync(userId, chatId, cutoffDate, token);

            if (token.IsCancellationRequested || messages == null || !messages.Any())
            {
                return new List<MessageDTO>();
            }

            return messages.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderID = m.SenderID,
                SenderUsername = m.IsSystemMessage ? "SYSTEM" : (m.Sender?.Username ?? "Unknown"),
                ChatID = m.ChatID,
                IsSystemMessage = m.IsSystemMessage,
            }).ToList();
        }
    }
}
