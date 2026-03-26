using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class SidebarService : ISidebarService
    {
        private readonly IContactService _contactService;
        private readonly IChatService _chatService;

        public SidebarService(IContactService contactService, IChatService chatService)
        {
            _contactService = contactService;
            _chatService = chatService;
        }

        public async Task<List<UserChatDTO>> GetSidebarItemsAsync(Guid userId)
        {
            try
            {
                var chatsTask = _chatService.GetUserChatListAsync(userId);
                var counterTask = _chatService.GetAllUnreadMessageCountsAsync(userId);

                await Task.WhenAll(chatsTask, counterTask);

                var chats = await chatsTask ?? new List<UserChatDTO>();
                var counters = await counterTask ?? new List<(Guid ChatId, int Count)>();

                var counterDict = counters.ToDictionary(t => t.ChatId, t => t.Count);

                var sidebarItems = chats.Select(c => new UserChatDTO
                {
                    ChatID = c.ChatID,
                    UserID = userId,
                    ChatName = c.ChatName,
                    IsArchive = c.IsArchive,
                    AvatarUrl = c.AvatarUrl,
                    UnreadMessageCount = counterDict.GetValueOrDefault(c.ChatID, 0),
                }).ToList();

                return sidebarItems;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
