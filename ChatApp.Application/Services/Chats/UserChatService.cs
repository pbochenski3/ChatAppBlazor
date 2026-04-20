using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services.Chats
{
    public class UserChatService : IUserChatService
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMediator _mediator;


        public UserChatService(IUserChatRepository userChatRepo, IMessageRepository messageRepo,IChatRepository chatRepo,IMediator mediator)
        {
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;
            _chatRepo = chatRepo;
            _mediator = mediator;
        }

        public async Task<List<UserChatDTO>> GetUserChatListAsync(Guid userId)
        {
            var chatEntries = await _userChatRepo.GetAllUserChatsAsync(userId);
          
            if (chatEntries == null || !chatEntries.Any())
                return new List<UserChatDTO>();

            var messageIds = chatEntries
            .Where(uc => uc.LastMessageID.HasValue)
            .Select(uc => uc.LastMessageID.Value)
            .Distinct()
            .ToList();

            var contentDict = await _messageRepo.GetMessagePreviewsAsync(messageIds);

            return chatEntries.Select(uc => new UserChatDTO
            {
                Identity = new ChatIdentityDTO
                {
                    ChatID = uc.ChatID,
                    ChatName = uc.ChatName,
                    IsGroup = uc.Chat.IsGroup,
                    AvatarUrl = uc.Chat.IsGroup
                        ? uc.Chat.AvatarUrl
                        : uc.Chat.UserChats.FirstOrDefault(p => p.UserID != userId)?.User?.AvatarUrl ?? "https://localhost:7255/cdn/avatars/default-avatar.png",
                    OtherUserId = uc.Chat.IsGroup ? null : uc.Chat.UserChats.FirstOrDefault(p => p.UserID != userId)?.UserID,
                    UserID = uc.UserID
                },
                State = new ChatStateDTO
                {
                    IsAdmin = uc.IsAdmin,
                    IsArchive = uc.IsArchive,
                    LastReadMessageID = uc.LastReadMessageID,
                    LastReadAt = uc.LastReadAt
                },
                LastMessage = new LastMessageDTO
                {
                    LastMessageID = uc.LastMessageID,
                    LastMessageContent = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var preview) ? preview.Content : null,
                    LastMessageSender = uc.LastMessageID.HasValue && contentDict.TryGetValue(uc.LastMessageID.Value, out var authorPreview) ? authorPreview.Author : null,
                    LastMessageAt = uc.LastMessageAt
                }
            }).ToList(); 
        }

        public async Task<UserChatDTO?> GetUserChatDetailsAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var chat = await _userChatRepo.GetUserChatAsync(chatId, userId, token);

            if (chat == null) return null;

            return new UserChatDTO
            {
                Identity = new ChatIdentityDTO
                {
                    ChatID = chat.ChatID,
                    ChatName = chat.ChatName,
                    IsGroup = chat.Chat.IsGroup,
                    AvatarUrl = chat.Chat.IsGroup
                        ? chat.Chat.AvatarUrl
                        : chat.Chat.UserChats.FirstOrDefault(p => p.UserID != userId)?.User?.AvatarUrl ?? "https://localhost:7255/cdn/avatars/default-avatar.png",
                    UserID = chat.UserID,
                    OtherUserId = chat.Chat.IsGroup ? null : chat.Chat.UserChats.FirstOrDefault(p => p.UserID != userId)?.UserID
                },
                State = new ChatStateDTO
                {
                    IsAdmin = chat.IsAdmin,
                    IsArchive = chat.IsArchive
                }
            };
        }

        public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId)
        {
            return await _userChatRepo.IsChatArchivedAsync(chatId, userId);
        }

        public async Task ArchiveUserChatAsync(Guid chatId, Guid userId)
        {
            await _userChatRepo.ArchiveChatAsync(chatId, userId);
        }
        public async Task UpdateChatNameAsync(Guid chatId, ChangeChatNameRequest request)
        {
            await _chatRepo.UpdateChatNameAsync(chatId, request.NewName);
            await _mediator.Publish(new ChatNameUpdatedNotification(chatId, request));
        }
    }
}
