using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class UserChatService : IUserChatService
    {
        private readonly IUserChatRepository _userChatRepo;
        public UserChatService(IUserChatRepository userChatRepo)
        {
           _userChatRepo = userChatRepo; 
        }
        public async Task MarkChatAsReadAsync(Guid userId, Guid chatId, Guid messageId)
        {
            await _userChatRepo.SaveLastReadMessage(userId, chatId, messageId);
        }
        public async Task SaveLastMessageAsync(Guid chatId, Guid messageId)
        {
            await _userChatRepo.SaveLastSendedChatMessage(chatId,messageId);
        }
        public async Task SaveLastSendedChatMessage(Guid chatId, Guid messageId)
        {
            await _userChatRepo.SaveLastSendedChatMessage(chatId, messageId);
        }
        public async Task<int> GetUnreadCounterAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.CountUnreadMessagesAsync(userId, chatId);
        }
        public async Task<List<CounterBadge>> GetAllUnreadCounterAsync(Guid userId)
        {
            return await _userChatRepo.CountAllUnreadMessagesAsync(userId);
        }
    }
}
