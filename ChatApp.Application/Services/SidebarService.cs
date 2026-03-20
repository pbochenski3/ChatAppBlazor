using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace ChatApp.Application.Services
{
    public class SidebarService : ISidebarService
    {
        private readonly IContactService _contactService;
        private readonly IChatService _chatService;
        private readonly IUserChatService _userChatService;
        public SidebarService(IContactService contactService,IChatService chatService,IUserChatService userChatService)
        {
            _contactService = contactService;
            _chatService = chatService;
            _userChatService = userChatService;
        }
        public async Task<List<UserChatDTO>> GetSidebarItems(Guid userId)
        {
            try
            {
                var chatsTask = _userChatService.GetChatList(userId);
                var counterTask = _userChatService.GetAllUnreadCounterAsync(userId);

                await Task.WhenAll(counterTask,chatsTask);

                var counter = await counterTask ?? new List<(Guid ChatId, int Count)>();
                var chats = await chatsTask ?? new List<UserChatDTO>();
                var counterDict = counter.ToDictionary(t => t.ChatId, t => t.Count);
                var sidebarItems = chats.Select(c => new UserChatDTO
                {
                    ChatID = c.ChatID,
                    UserID = userId,
                    ChatName = c.ChatName,
                    IsArchive = c.IsArchive,
                    counter = counterDict.GetValueOrDefault(c.ChatID,0),
                }).ToList();
                return sidebarItems;

      
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
