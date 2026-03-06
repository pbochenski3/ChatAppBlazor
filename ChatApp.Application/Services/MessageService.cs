using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;

        public MessageService(IMessageRepository messageRepo,IUserRepository userRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
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
                Sender = sender,
                ChatID = Guid.Parse("018F10B5-A3D4-7B3F-8E12-3C4D5E6F7A8B")

            };
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
        }
        public async Task<List<MessageDTO>> GetMessagesHistoryAsync(int count)
        {
            var howMany = 15;
            var rawMessage = await _messageRepo.GetRecentMessagesAsync(count);
            return rawMessage.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderID = m.Sender.UserID,
                SenderUsername = m.Sender.Username,
                ChatID = m.ChatID
            }).ToList();
        }
    }
}
