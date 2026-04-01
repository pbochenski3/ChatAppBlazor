using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services.Chats
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;

        public PrivateChatService(IChatRepository chatRepo, IUserRepository userRepo, IUserChatRepository userChatRepo,IMessageRepository messageRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;
        }

        public async Task CreatePrivateChatAsync(Guid userId1, Guid userId2)
        {
            var chat = await _chatRepo.GetChatAsync(userId1, userId2);

            if (chat == null)
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
                var systemMessage = new Message
                {
                    MessageID = Guid.CreateVersion7(),
                    ChatID = newChat.ChatID,
                    Content = "Ten czat nie ma jeszcze wiadomości! Przywitaj się!",
                    SentAt = DateTime.UtcNow,
                    IsSystemMessage = true,
                };
                await _messageRepo.AddMessageAsync(systemMessage);
            }
            else
            {
                await _userChatRepo.RestoreChatAsync(chat.ChatID);
            }
        }

        public async Task<Guid?> GetPrivateChatIdAsync(Guid userId, Guid contactUserId, CancellationToken token)
        {
            var chat =  await _chatRepo.GetChatAsync(userId, contactUserId, token);
            return chat.ChatID;
        }

        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            return await _userChatRepo.GetReceiverUserIdAsync(chatId, userId, token);
        }
    }
}
