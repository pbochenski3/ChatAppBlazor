using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;

        public MessageService(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
        }
        public async Task SendMessageAsync(MessageDTO dto)
        {
            var message = new Message
            {
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                SenderID = dto.SenderID,
                ChatID = dto.ChatID
            };
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
        }
    }
}
