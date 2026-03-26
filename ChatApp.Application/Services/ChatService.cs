using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
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

        public async Task<ChatDTO> GetChatDetailsAsync(Guid chatId)
        {
            var chat = await _chatRepo.FetchChatById(chatId);
            if (chat == null) return null!;

            return new ChatDTO
            {
                ChatID = chat.ChatID,
                ChatName = chat.ChatName,
                AvatarUrl = chat.AvatarUrl,
            };
        }

        public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
        {
            return await _userChatRepo.GetUsersInChatAsync(chatId);
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
