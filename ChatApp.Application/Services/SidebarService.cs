using ChatApp.Application.DTO;
using ChatApp.Application.Mappers;
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
        public async Task<List<SidebarDTO>> GetSidebarItems(Guid id)
        {
            var contactsTask = _contactService.GetUserContactsAsync(id);
            var chatsTask = _chatService.GetChatList(id);
            var counterTask =  _userChatService.GetAllUnreadCounterAsync(id);
            await Task.WhenAll(counterTask, contactsTask, chatsTask);
            var counterBadge = await counterTask ?? new List<CounterBadge>();
            var contacts = await contactsTask ?? new List<ContactDTO>();
            var chats = await chatsTask ?? new List<ChatDTO>();
            var counterDict = counterBadge.ToDictionary(c => c.Id, c => c.Counter);
            var chatItems = chats.Select(ch =>
            ch.MapToSidebar(counterDict.GetValueOrDefault(ch.ChatID, 0))).ToList();
            var contactItems = contacts.Select(c =>
            c.MapToSidebar(counterDict.GetValueOrDefault(c.ContactUserID, 0))).ToList();
            return chatItems
                .Concat(contactItems)
                .OrderByDescending(c => c.Counter > 0)
                .ToList();
        }
    }
}
