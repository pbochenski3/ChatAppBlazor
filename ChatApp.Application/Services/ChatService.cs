using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IContactRepository _contactRepo;
        public ChatService(IChatRepository chatRepo, IUserRepository userRepo, IContactRepository contactRepo, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
            _contactRepo = contactRepo;
        }

        public async Task<bool> GetChatStatus(Guid ChatId, Guid ContactId)
        {
            return await _chatRepo.GetChatStatusById(ChatId, ContactId);
        }

    }
}
