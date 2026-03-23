 using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Application.Services
{
    public class UserChatService : IUserChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IContactRepository _contactRepo;
        public UserChatService
            (IUserChatRepository userChatRepo,
            IChatRepository chatRepo,
            IUserRepository userRepo,
            IContactRepository contactRepo
            )
        {
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
            _chatRepo = chatRepo;
            _contactRepo = contactRepo;
        }
        public async Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId)
        {
            await _userChatRepo.SaveLastReadMessage(userId, chatId, messageId);
        }
        public async Task<UserChatDTO> GetChatAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var chat = await _userChatRepo.FetchChatAsync(chatId, userId, token);

            if (chat == null) return null;

            var contactId = await _userChatRepo.FetchReceiverUser(chatId, userId, token);
            if (contactId == Guid.Empty) return null;

            bool isFriend = chat.IsArchive;
                //|| await _contactRepo.CheckIfContact(userId, contactId, token);

            if (isFriend) return null;

          
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

        public async Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId,CancellationToken token)
        {
            await _userChatRepo.SaveChatAsReaded(userId, chatId,token);
        }
        public async Task SaveLastSendedChatMessageAsync(Guid chatId, Guid messageId)
        {
            await _userChatRepo.SaveLastSendedChatMessage(chatId, messageId);
        }
        public async Task<int> GetUnreadCounterAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.CountUnreadMessagesAsync(userId, chatId);
        }
        public async Task<List<(Guid ChatId, int Count)>> GetAllUnreadCounterAsync(Guid userId)
        {
            return await _userChatRepo.CountAllUnreadMessagesAsync(userId);
        }
        public async Task<Guid> GetReceiverUser(Guid chatId, Guid userId,CancellationToken token)
        {
            return await _userChatRepo.FetchReceiverUser(chatId, userId,token);
        }
        public async Task<Guid> GetChatId(Guid userId, Guid contactUserId, CancellationToken token)
        {
            return await _chatRepo.GetChatIdAsync(userId, contactUserId,token);
        }
        public async Task<HashSet<Guid>> GetListOfUsersInChatAsync(Guid chatId)
        {
            return await _userChatRepo.FetchUsersInChatAsync(chatId);

        }
        public async Task<bool> GetGroupChatByIdAsync(Guid chatId )
        {
            return await _userChatRepo.CheckIfChatExisted(chatId);
        }
        public async Task AddUserGroupToDb(Guid chatId,HashSet<Guid> usersToAdd)
        {
            await _chatRepo.AddUserGroupToDb(chatId, usersToAdd);
        }

        public async Task CreateGroupChat(Guid chatId, HashSet<Guid> UsersToAdd)
        {
            HashSet<Guid> UsersInGroup = new HashSet<Guid>();
            UsersInGroup = await _userChatRepo.FetchUsersInChatAsync(chatId);
            UsersInGroup.UnionWith(UsersToAdd);
            int number = RandomNumberGenerator.GetInt32(0, 100000);
            var newChat = new Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = $"Chat#{number:D5}",
                UserChats = new List<UserChat>()
            };
            foreach (var user in UsersInGroup)
            {
                newChat.UserChats.Add(new UserChat
                {
                    UserID = user,
                    ChatID = newChat.ChatID,
                    IsArchive = false,
                });
            }
            await _chatRepo.AddChatAsync(newChat);

        }
        public async Task CreatePrivateChat(Guid user1, Guid user2)
        {
            var chatId = await _chatRepo.GetChatIdAsync(user1, user2);
            var check = await _userChatRepo.CheckIfChatExisted(chatId);
            if(!check)
            { 
            var userModel1 = await _userRepo.GetByIdAsync(user1);
            var userModel2 = await _userRepo.GetByIdAsync(user2);
                var newChat = new Chat
                {
                    ChatID = Guid.CreateVersion7(),
                    CreatedAt = DateTime.UtcNow,
                    IsGroup = false,
                    UserChats = new List<UserChat>(),

                };
                newChat.UserChats.Add(new UserChat
                {
                    UserID = user1,
                    ChatID = newChat.ChatID,
                    ChatName = userModel2.Username,
                    IsArchive = false,
                });
            newChat.UserChats.Add(new UserChat
            {
                UserID = user2,
                ChatID = newChat.ChatID,
                ChatName = userModel1.Username,
                IsArchive = false,
            });
            await _chatRepo.AddChatAsync(newChat);
        }
            else
            {

                await _userChatRepo.RestoreChat(chatId);
            }
        }
        public async Task<List<UserChatDTO>> GetChatList(Guid userId)
        {
            var chatEntries = await _userChatRepo.FetchAllChatsAsync(userId);

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
    }
}
