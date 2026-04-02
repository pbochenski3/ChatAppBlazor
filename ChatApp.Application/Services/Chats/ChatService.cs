using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Services.Chats
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;

        public ChatService(IChatRepository chatRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
        }
        public async Task UpdateGroupAvatarUrl(Guid chatId,string avatarUrl)
        {
        
            await _chatRepo.UpdateGroupAvatarUrl(chatId, avatarUrl);
        }

        public async Task<HashSet<Guid>> GetUsersInChatIdAsync(Guid chatId)
        {
            return await _userChatRepo.GetUsersInChatIdAsync(chatId);
        }
        public async Task<string> GetGroupAvatarUrlAsync(Guid chatId)
        {
             return await _chatRepo.GetGroupAvatarUrlAsync(chatId);
        }
        public async Task DeleteChatAsync(Guid chatId, Guid userId)
        {
            await _userChatRepo.MarkChatAsDeletedAsync(chatId, userId);
            await _chatRepo.TryDeleteChatIfEmptyAsync(chatId);
        }

        public async Task<bool> IsGroupChatExistingAsync(Guid chatId, Guid userId)
        {
            return await _chatRepo.CheckIfGroupExist(chatId, userId);
        }
    }
}
