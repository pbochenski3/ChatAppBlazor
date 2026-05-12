using ChatApp.Application.DTO.Chats;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Projections
{
    public static class UserChatProjections
    {
        public static IQueryable<UserChatDTO> ProjectToSidebar(this IQueryable<UserChat> query, Guid currentUserId)
        {
            return query.Select(uc => new UserChatDTO
            {
                Identity = new ChatIdentityDTO
                {
                    ChatID = uc.ChatID,
                    ChatName = uc.Chat.IsGroup
                        ? (uc.Chat.ChatName ?? "Grupa")
                        : (uc.Alias ?? uc.Chat.UserChats
                            .Where(p => p.UserID != currentUserId)
                            .Select(p => p.User.Username)
                            .FirstOrDefault() ?? "Użytkownik"),
                    Alias = uc.Alias ?? "Nieznany",
                    IsGroup = uc.Chat.IsGroup,
                    AvatarUrl = uc.Chat.IsGroup
                        ? (uc.Chat.AvatarUrl ?? "default-group.png")
                        : (uc.Chat.UserChats
                            .Where(p => p.UserID != currentUserId)
                            .Select(p => p.User.AvatarUrl)
                            .FirstOrDefault() ?? "default-avatar.png"),
                    OtherUserId = uc.Chat.IsGroup ? null : (Guid?)uc.Chat.UserChats
                        .Where(p => p.UserID != currentUserId)
                        .Select(p => p.UserID)
                        .FirstOrDefault(),
                    UserID = uc.UserID
                },
                State = new ChatStateDTO
                {
                    IsAdmin = uc.IsAdmin,
                    IsArchive = uc.IsArchive,
                    IsDeleted = uc.IsDeleted,
                    LastReadMessageID = uc.LastReadMessageID,
                    UnreadMessageCount = uc.Chat.Messages
                        .Count(m => m.SentAt > uc.LastReadAt && m.SenderID != currentUserId)
                },
                LastMessage = new LastMessageDTO
                {
                    LastMessageID = uc.Chat.LastMessageID,
                    LastMessageContent = uc.Chat.LastMessage != null
                        ? uc.Chat.LastMessage.Content
                        : "Brak wiadomości",
                    LastMessageSender = uc.Chat.LastMessage != null
                        ? uc.Chat.LastMessage.Sender.Username
                        : "System",
                    LastMessageAt = uc.Chat.LastMessageAt,
                    LastMessageSenderId = uc.Chat.LastMessage != null
                        ? uc.Chat.LastMessage.SenderID
                        : null
                }
            });
        }
    }
}
