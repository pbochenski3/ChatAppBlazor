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
        public async Task SendChatMessageAsync(MessageDTO dto)
        {
            var sender = await _userRepo.GetByIdAsync(dto.SenderID);
            if(sender == null)
            {
                throw new Exception($"User: {dto.SenderID} not found");
            }
            else if(string.IsNullOrWhiteSpace(dto.Content))
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
            };
            await _messageRepo.AddAsync(message);
           
        }
           
        public async Task<List<MessageDTO>> GetPrivateHistoryAsync(Guid userId,Guid chatId,CancellationToken token)
        {

            var rawMessage = await _messageRepo.GetMessageHistoryAsync(userId,chatId,token);
            if(!rawMessage.Any()) { return new List<MessageDTO>(); };
            if (token.IsCancellationRequested) return new List<MessageDTO>();
            return rawMessage.Select(uc => new MessageDTO
                {
                    MessageID = uc.MessageID,
                    Content = uc.Content,
                    SentAt = uc.SentAt,
                    SenderID = uc.SenderID,
                    SenderUsername = uc.Sender.Username,
                    ChatID = chatId,
                }).ToList();
            }
            
        }
    }
