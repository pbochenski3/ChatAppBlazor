using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ChatApp.Domain.Repository;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IChatService _chatService;
        private readonly IUserChatRepository _userChatRepository;

        public MessageService(IMessageRepository messageRepo,
            IUserRepository userRepo,
            IChatService chatService,
            IUserChatRepository userChatRepository
            )
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _chatService = chatService;
            _userChatRepository = userChatRepository;
        }
        public async Task SaveChatMessageAsync(MessageDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new Exception("Message content cannot be empty");
            }
            var message = new Message
            {
                Content = dto.Content,
                SenderID = dto.SenderID,
                ChatID = dto.ChatID,
                MessageID = dto.MessageID,
                SentAt = dto.SentAt,
                IsSystemMessage = dto.IsSystemMessage,
            };
            await _messageRepo.AddAsync(message);

        }

        public async Task<List<MessageDTO>> GetPrivateHistoryAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            bool isArchive = await _chatService.CheckIfUserChatIsArchiveAsync(chatId, userId);
            DateTime? cutoffDate = null;

            if (isArchive)
            { 
                cutoffDate = await _chatService.GetLastSeenMessage(userId, chatId);
            }

            var messages = await _messageRepo.GetMessageHistoryAsync(userId, chatId, cutoffDate, token);

            if (token.IsCancellationRequested || messages == null || !messages.Any())
            {
                return new List<MessageDTO>();
            }
            return messages.Select(uc => new MessageDTO
            {
                MessageID = uc.MessageID,
                Content = uc.Content,
                SentAt = uc.SentAt,
                SenderID = uc.SenderID,
                SenderUsername = uc.IsSystemMessage ? "SYSTEM" : (uc.Sender?.Username ?? "Unknown"),
                ChatID = uc.ChatID,
                IsSystemMessage = uc.IsSystemMessage,
            }).ToList();
        }
    }
}