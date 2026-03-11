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
        public async Task<ChatDTO> GetChatById(Guid contactId, Guid currenUserId)
        {
            var chat = await _chatRepo.GetChatById(contactId, currenUserId);
          if(chat == null)
            {
                await CreateChat(contactId, currenUserId);
                chat = await _chatRepo.GetChatById(contactId, currenUserId);
            }
            return new ChatDTO
            {
                ChatID = chat.ChatID,
                CreatedAt = chat.CreatedAt,
                ChatName = chat.ChatName
            };
        }
        public async Task CreateChat(Guid contactId, Guid id)
        {
            var user1 = await _userRepo.GetByIdAsync(id);
            var user2 = await _userRepo.GetByIdAsync(contactId);
            if(user1 == null || user2 == null)
{
                throw new Exception("One of the users don`t exist");
            }

            var chatName = $"{user1.Username} & {user2.Username} Chat";
            var newChat = new Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                ChatName = chatName

            };
            newChat.UserChats.Add(new UserChat
            {
                UserID = user2.UserID,
                ChatID = newChat.ChatID
            });
            newChat.UserChats.Add(new UserChat
            {
                UserID = user1.UserID,
                ChatID = newChat.ChatID
            });
            await _chatRepo.AddChatAsync(newChat);
        }

    }
}
