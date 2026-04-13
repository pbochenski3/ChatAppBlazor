using ChatApp.Application.Interfaces.Chats;
using ChatApp.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services.Chats
{
    public class ChatReadStatusService : IChatReadStatusService
    {
        private readonly IUserChatRepository _userChatRepo;

        public ChatReadStatusService(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }

        public async Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId)
        {
            await _userChatRepo.UpdateLastReadMessageAsync(userId, chatId, messageId);
        }

        public async Task MarkAllMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            var userChat = await _userChatRepo.GetUserChatAsync(chatId, userId, token);
            var lastMessageId = userChat?.LastMessageID;
            if (lastMessageId.HasValue)
            {
                await _userChatRepo.UpdateLastReadMessageAsync(userId, chatId, lastMessageId.Value);
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.CountUnreadMessagesAsync(userId, chatId);
        }

        public async Task<List<(Guid ChatId, int Count)>> GetAllUnreadMessageCountsAsync(Guid userId)
        {
            return await _userChatRepo.CountAllUnreadMessageCountsAsync(userId);
        }

        public async Task<DateTime?> GetLastMessageAtChatAsync(Guid userId, Guid chatId)
        {
            return await _userChatRepo.GetLastMessageDateAsync(userId, chatId);
        }

        public async Task SaveLastSentMessageIdAsync(Guid chatId, Guid messageId)
        {
            await _userChatRepo.UpdateLastSentMessageAsync(chatId, messageId);
        }
    }
}
