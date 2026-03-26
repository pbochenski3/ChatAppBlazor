using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;

        public PrivateChatService(IChatRepository chatRepo, IUserRepository userRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
        }

        public async Task CreatePrivateChatAsync(Guid userId1, Guid userId2)
        {
            var chatId = await _chatRepo.GetChatIdAsync(userId1, userId2);
            var exists = await _userChatRepo.ExistsAsync(chatId);

            if (!exists)
            {
                var user1 = await _userRepo.GetByIdAsync(userId1);
                var user2 = await _userRepo.GetByIdAsync(userId2);

                var newChat = new Chat
                {
                    ChatID = Guid.CreateVersion7(),
                    CreatedAt = DateTime.UtcNow,
                    IsGroup = false,
                    UserChats = new List<UserChat>(),
                };

                newChat.UserChats.Add(new UserChat
                {
                    UserID = userId1,
                    ChatID = newChat.ChatID,
                    ChatName = user2.Username,
                    IsArchive = false,
                });

                newChat.UserChats.Add(new UserChat
                {
                    UserID = userId2,
                    ChatID = newChat.ChatID,
                    ChatName = user1.Username,
                    IsArchive = false,
                });

                await _chatRepo.AddChatAsync(newChat);
            }
            else
            {
                await _userChatRepo.RestoreChatAsync(chatId);
            }
        }

        public async Task<Guid> GetPrivateChatIdAsync(Guid userId, Guid contactUserId, CancellationToken token)
        {
            return await _chatRepo.GetChatIdAsync(userId, contactUserId, token);
        }

        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            return await _userChatRepo.GetReceiverUserIdAsync(chatId, userId, token);
        }
    }
}
