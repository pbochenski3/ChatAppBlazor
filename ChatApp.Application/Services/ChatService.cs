using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
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
        private readonly IContactRepository _contactRepo;
        public ChatService(IChatRepository chatRepo, IUserRepository userRepo, IContactRepository contactRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _contactRepo = contactRepo;
        }
        public async Task<ChatDTO> GetPrivateChatById(Guid Id, Guid currenUserId)
        {
            var chat = await _chatRepo.GetPrivateChatC(Id, currenUserId);
      

            if (chat == null)
            {
                var user1Task =  _userRepo.GetByIdAsync(Id);
                var user2Task =  _userRepo.GetByIdAsync(currenUserId);

                await Task.WhenAll(user1Task, user2Task);
                var user1 = user1Task.Result;
                var user2 = user2Task.Result;

                if (user1 == null || user2 == null) throw new Exception("User not found");

                var users = new List<User> { user1, user2 };
                chat = await CreateChat(users, $"{user1.Username} && {user2.Username} Chat", false);
            }
            else
            {
                var myUserChat = chat.UserChats.FirstOrDefault(uc => uc.UserID == currenUserId);

                var otherUserInChat = chat.UserChats.FirstOrDefault(uc => uc.UserID != currenUserId);

                if (otherUserInChat != null)
                {
                    var deletedContactRecord = await _contactRepo.CheckContact(currenUserId, otherUserInChat.UserID);

                    if (myUserChat != null && myUserChat.IsArchive && deletedContactRecord == null)
                    {
                        await _chatRepo.RestoreChat(chat.ChatID);
                        myUserChat.IsArchive = false;
                        myUserChat.ArchivedAt = null;
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
        public async Task<bool> GetChatStatus(Guid ChatId, Guid ContactId)
        {
            return await _chatRepo.GetChatStatusById(ChatId, ContactId);
        }
        public async Task<List<ChatDTO>> GetChatList (Guid UserId)
        {
            var ChatList =  await _chatRepo.GetChatListFromDb(UserId);
            if(!ChatList.Any())
            {
                return new List<ChatDTO>();
            }
            return ChatList.Select(chat =>
            {
                return new ChatDTO
                {
                    ChatID = chat.ChatID,
                    IsArchive = chat.IsArchive,
                    ChatName = chat.Chat.ChatName,
                    IsGroup = chat.Chat.IsGroup,
                };
            })
                .ToList();
        }

    }
}
