using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
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
                var chats = await _userChatService.GetUserChatListAsync(userId);
                var counters = await _readStatusService.GetAllUnreadMessageCountsAsync(userId);



                var counterDict = counters.ToDictionary(t => t.ChatId, t => t.Count);

                var sidebarItems = chats.Select(c => new UserChatDTO
                {
                    Identity = new ChatIdentityDTO
                    {
                        ChatID = c.Identity.ChatID,
                        ChatName = c.Identity.ChatName,
                        AvatarUrl = c.Identity.AvatarUrl,
                        IsGroup = c.Identity.IsGroup,
                        UserID = c.Identity.UserID,
                        OtherUserId = c.Identity.OtherUserId
                    },
                    State = new ChatStateDTO
                    {
                        IsArchive = c.State.IsArchive,
                        IsAdmin = c.State.IsAdmin,
                        IsDeleted = c.State.IsDeleted,
                        LastReadMessageID = c.State.LastReadMessageID,
                        LastReadAt = c.State.LastReadAt,
                        UnreadMessageCount = counterDict.GetValueOrDefault(c.Identity.ChatID, 0)
                    },
                    LastMessage = new LastMessageDTO
                    {
                        LastMessageID = c.LastMessage.LastMessageID,
                        LastMessageContent = c.LastMessage.LastMessageContent,
                        LastMessageSender = c.LastMessage.LastMessageSender,
                        LastMessageAt = c.LastMessage.LastMessageAt
                    }
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
