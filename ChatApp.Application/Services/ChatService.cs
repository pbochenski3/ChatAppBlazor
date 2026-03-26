using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;

        public ChatService(IChatRepository chatRepo, IUserRepository userRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
        }

        public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId)
        {
            return await _userChatRepo.IsChatArchivedAsync(chatId, userId);
        }

        public async Task<bool> IsGroupChatExistingAsync(Guid chatId, Guid userId)
        {
            return await _chatRepo.CheckIfGroupExist(chatId, userId);
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

        public async Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId)
        {
            await _userChatRepo.UpdateLastReadMessageAsync(userId, chatId, messageId);
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

        public async Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            await _userChatRepo.MarkChatAsReadAsync(userId, chatId, token);
        }

        public async Task SaveLastSentMessageIdAsync(Guid chatId, Guid messageId)
        {
            await _userChatRepo.UpdateLastSentMessageAsync(chatId, messageId);
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.CountUnreadMessagesAsync(userId, chatId);
        }

        public async Task<List<(Guid ChatId, int Count)>> GetAllUnreadMessageCountsAsync(Guid userId)
        {
            return await _userChatRepo.CountAllUnreadMessageCountsAsync(userId);
        }

        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            return await _userChatRepo.GetReceiverUserIdAsync(chatId, userId, token);
        }

        public async Task<Guid> GetPrivateChatIdAsync(Guid userId, Guid contactUserId, CancellationToken token)
        {
            return await _chatRepo.GetChatIdAsync(userId, contactUserId, token);
        }

        public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
        {
            return await _userChatRepo.GetUsersInChatAsync(chatId);
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

            int number = RandomNumberGenerator.GetInt32(0, 100000);
            var newChat = new Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = $"Chat#{number:D5}",
                UserChats = new List<UserChat>()
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

        public async Task<List<UserChatDTO>> GetUserChatListAsync(Guid userId)
        {
            var chatEntries = await _userChatRepo.GetAllUserChatsAsync(userId);

            if (chatEntries == null || !chatEntries.Any())
                return new List<UserChatDTO>();

            return chatEntries.Select(uc => new UserChatDTO
            {
                ChatID = uc.ChatID,
                UserID = uc.UserID,
                ChatName = uc.ChatName,
                IsArchive = uc.IsArchive,
                IsAdmin = uc.IsAdmin,
                JoinedAt = uc.JoinedAt,
            }).ToList();
        }

        public async Task ArchiveUserChatAsync(Guid chatId, Guid userId)
        {
            await _userChatRepo.ArchiveChatAsync(chatId, userId);
        }

        public async Task<DateTime?> GetLastSeenMessageAtAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.GetLastReadAtAsync(userId, chatId);
        }

        public async Task DeleteChatAsync(Guid chatId, Guid userId)
        {
            await _userChatRepo.MarkChatAsDeletedAsync(chatId, userId);
            await _chatRepo.TryDeleteChatIfEmptyAsync(chatId);
        }
    }
}
