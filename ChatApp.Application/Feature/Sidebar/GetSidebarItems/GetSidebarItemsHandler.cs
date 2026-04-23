using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Sidebar.GetSidebarItems
{
    public class GetSidebarItemsHandler : IRequestHandler<GetSidebarItemsQuery, List<UserChatDTO>>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        public GetSidebarItemsHandler(IUserChatRepository userChatRepo, IMessageRepository messageRepo)
        {
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;

        }

        public async Task<List<UserChatDTO>> Handle(GetSidebarItemsQuery r, CancellationToken cancellationToken)
        {
            var chatEntries = await _userChatRepo.GetAllUserChatsAsync(r.UserId);
            if (chatEntries == null || !chatEntries.Any())
                return new List<UserChatDTO>();
            var counters = await _userChatRepo.CountAllUnreadMessageCountsAsync(r.UserId);
            var counterDict = counters.ToDictionary(t => t.ChatId, t => t.Count);
            var messageIds = chatEntries
              .Where(uc => uc.LastMessageID.HasValue)
              .Select(uc => uc.LastMessageID.Value)
              .Distinct()
              .ToList();

            var contentDict = await _messageRepo.GetMessagePreviewsAsync(messageIds);
            return chatEntries
          .Select(uc => MapToUserChatDto(uc, r.UserId, contentDict, counterDict))
          .ToList();
            }


        private UserChatDTO MapToUserChatDto(
         UserChat uc,
         Guid currentUserId,
         Dictionary<Guid, MessagePreview> contentDict,
         Dictionary<Guid, int> unreadCounters)
        {
            var otherUser = !uc.Chat.IsGroup
                ? uc.Chat.UserChats.FirstOrDefault(p => p.UserID != currentUserId)?.User
                : null;
            var otherUserChat = otherUser?.UserChats
            .FirstOrDefault(x => x.ChatID == uc.ChatID);

            return new UserChatDTO
            {
                Identity = new ChatIdentityDTO
                {
                    ChatID = uc.ChatID,
                    ChatName = uc.Chat.IsGroup
                        ? (string.IsNullOrWhiteSpace(uc.ChatName) ? uc.Chat.ChatName : uc.ChatName)
                        : $"{otherUserChat?.ChatName}".Trim(),
                    IsGroup = uc.Chat.IsGroup,
                    AvatarUrl = uc.Chat.IsGroup
                        ? (uc.Chat.AvatarUrl ?? "https://localhost:7255/cdn/avatars/default-group.png")
                        : (otherUser?.AvatarUrl ?? "https://localhost:7255/cdn/avatars/default-avatar.png"),
                    OtherUserId = otherUser?.UserID,
                    UserID = uc.UserID
                },
                State = new ChatStateDTO
                {
                    IsAdmin = uc.IsAdmin,
                    IsArchive = uc.IsArchive,
                    IsDeleted = uc.IsDeleted,
                    LastReadMessageID = uc.LastReadMessageID,
                    LastReadAt = uc.LastReadAt,
                    UnreadMessageCount = unreadCounters.GetValueOrDefault(uc.ChatID, 0)
                },
                LastMessage = new LastMessageDTO
                {
                    LastMessageID = uc.LastMessageID,
                    LastMessageContent = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var preview)
                        ? preview.Content
                        : "Brak wiadomości",
                    LastMessageSender = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var authorPreview)
                        ? authorPreview.Author
                        : null,
                    LastMessageAt = uc.LastMessageAt
                }
            };
        }
    }
}
