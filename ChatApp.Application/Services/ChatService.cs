using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
            public ChatService(IChatRepository chatRepo,IUserRepository userRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }
        public async Task<ChatDTO> GetPrivateChatById(Guid contactId, Guid currenUserId)
        {
            var chat = await _chatRepo.GetPrivateChat(contactId, currenUserId);

            if (chat == null)
            {
                var user1 = await _userRepo.GetByIdAsync(contactId);
                var user2 = await _userRepo.GetByIdAsync(currenUserId);

                if (user1 == null || user2 == null) throw new Exception("User not found");

                var users = new List<User> { user1, user2 };
                chat = await CreateChat(users, $"{user1.Username} && {user2.Username} Chat", false);
            }
            else
            { 
                var myChatStatus = chat.UserChats.FirstOrDefault(uc => uc.UserID == currenUserId);

                if (myChatStatus != null && (myChatStatus.IsDeleted || myChatStatus.IsArchive))
                {
                    await _chatRepo.RestoreChat(chat.ChatID);

                    foreach (var uc in chat.UserChats)
                    {
                        uc.IsDeleted = false;
                        uc.DeletedAt = null;
                        uc.IsArchive = false;
                    }
                }
            }

            return new ChatDTO
            {
                ChatID = chat.ChatID,
                CreatedAt = chat.CreatedAt,
                ChatName = chat.ChatName,
                IsArchive = chat.UserChats.FirstOrDefault(uc => uc.UserID == currenUserId)?.IsArchive ?? false
            };
        }
        public async Task<Chat> CreateChat(List<User> users,string ChatName,bool isGroup)
        {
            if(!users.Any())
{
                throw new Exception("List is empty");
            }
            var newChat = new Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                ChatName = ChatName,
                IsGroup = isGroup

            };
            foreach (var user in users)
            {
                newChat.UserChats.Add(new UserChat
                {
                    UserID = user.UserID,
                    ChatID = newChat.ChatID
                });
            }
            await _chatRepo.AddChatAsync(newChat);
            return newChat;
        }

    }
}
