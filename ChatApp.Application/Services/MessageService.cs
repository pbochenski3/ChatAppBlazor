using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
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
        private readonly IUserRepository _userRepo;
        private readonly IChatService _chatService;
        private readonly IUserChatRepository _userChatRepo;

        public MessageService(IMessageRepository messageRepo,
            IUserRepository userRepo,
            IChatService chatService,
            IUserChatRepository userChatRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _chatService = chatService;
            _userChatRepo = userChatRepo;
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
            await _messageRepo.AddAsync(message);
        }

        public async Task<List<MessageDTO>> GetChatHistoryAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            bool isArchive = await _chatService.IsChatArchivedAsync(chatId, userId);
            DateTime? cutoffDate = null;

            if (isArchive)
            {
                cutoffDate = await _chatService.GetLastSeenMessageAtAsync(userId, chatId);
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
