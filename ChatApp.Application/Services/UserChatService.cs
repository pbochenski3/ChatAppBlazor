using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class UserChatService : IUserChatService
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;

        public UserChatService(IUserChatRepository userChatRepo, IMessageRepository messageRepo)
        {
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;
        }

        public async Task<List<UserChatDTO>> GetUserChatListAsync(Guid userId)
        {
            var chatEntries = await _userChatRepo.GetAllUserChatsAsync(userId);
            var messageIds = chatEntries
                .Where(uc => uc.LastMessageID.HasValue) 
                .Select(uc => uc.LastMessageID.Value)   
                .Distinct()                                 
                .ToList();
            var contentDict = await _messageRepo.GetMessagePreviewsAsync(messageIds);

            if (chatEntries == null || !chatEntries.Any())
                return new List<UserChatDTO>();

            return chatEntries.Select(uc => new UserChatDTO
            {
                ChatID = uc.ChatID,
                UserID = uc.UserID,
                ChatName = uc.ChatName,
                IsArchive = uc.IsArchive,
                IsAdmin = uc.IsAdmin,

                LastMessageContent = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var preview)
                    ? preview.Content
                    : null,
                LastMessageSender = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var authorPreview)
                    ? authorPreview.Author
                    : null,
                IsGroup = uc.Chat.IsGroup,
               
            }).ToList(); 
        }

        public async Task<UserChatDTO?> GetUserChatDetailsAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var chat = await _userChatRepo.GetUserChatAsync(chatId, userId, token);

            if (chat == null) return null;

            return new UserChatDTO
            {
                ChatID = chat.ChatID,
                UserID = chat.UserID,
                ChatName = chat.ChatName,
                IsAdmin = chat.IsAdmin,
                IsArchive = chat.IsArchive,
                IsGroup = chat.Chat.IsGroup,
            };
        }

        public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId)
        {
            return await _userChatRepo.IsChatArchivedAsync(chatId, userId);
        }

        public async Task ArchiveUserChatAsync(Guid chatId, Guid userId)
        {
            await _userChatRepo.ArchiveChatAsync(chatId, userId);
        }
        public async Task UpdateChatNameAsync(Guid chatId, string chatName)
        {
            await _userChatRepo.UpdateChatNameAsync(chatId, chatName);
        }
    }
}
