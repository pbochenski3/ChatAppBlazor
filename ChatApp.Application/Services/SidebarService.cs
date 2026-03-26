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
        private readonly IUserChatService _userChatService;
        private readonly IChatReadStatusService _readStatusService;

        public SidebarService(IUserChatService userChatService, IChatReadStatusService readStatusService)
        {
            _userChatService = userChatService;
            _readStatusService = readStatusService;
        }

        public async Task<List<UserChatDTO>> GetSidebarItemsAsync(Guid userId)
        {
            try
            {
                var chatsTask = _userChatService.GetUserChatListAsync(userId);
                var counterTask = _readStatusService.GetAllUnreadMessageCountsAsync(userId);

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
