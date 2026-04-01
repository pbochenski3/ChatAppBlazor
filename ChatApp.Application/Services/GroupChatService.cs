using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ChatApp.Application.Services
{
    public class GroupChatService : IGroupChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;

        public GroupChatService(IChatRepository chatRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
        }

        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            var usersWithHistory = await _chatRepo.GetExistingUsersInChat(chatId, userIdsToAdd);
            if (usersWithHistory != null && usersWithHistory.Any())
            {
                await _userChatRepo.RestoreGroupChatForUsersAsync(chatId, usersWithHistory);
                var usersWithoutHistory = userIdsToAdd.Where(id => !usersWithHistory.Contains(id)).ToHashSet();
                if (usersWithoutHistory.Any())
                {
                    await _chatRepo.AddUserGroupToDb(chatId, usersWithoutHistory);
                }
            }
            else
            {
                await _chatRepo.AddUserGroupToDb(chatId, userIdsToAdd);
            }
        }

        public async Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd)
        {
            var usersInGroup = await _userChatRepo.GetUsersInChatAsync(existingChatId);
            usersInGroup.UnionWith(userIdsToAdd);
            if(usersInGroup.Count < 3)
            {
                throw new InvalidOperationException("Chat group need more that 2 people!");
            }

            int number = RandomNumberGenerator.GetInt32(0, 100000);
            var newChat = new Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = $"Chat#{number:D5}",
                UserChats = new List<UserChat>(),
                AvatarUrl = "https://localhost:7255/cdn/avatars/default-group-avatar.png"
            };

            foreach (var userId in usersInGroup)
            {
                newChat.UserChats.Add(new UserChat
                {
                    UserID = userId,
                    ChatID = newChat.ChatID,
                    ChatName = newChat.ChatName,
                    IsArchive = false,
                });
            }

            await _chatRepo.AddChatAsync(newChat);
            return newChat.ChatID;
        }
    }
}
