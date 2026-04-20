using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace ChatApp.Application.Services.Chats
{
    public class ChatService : IChatService
    {
        private readonly ITransactionProvider _transactionProvider;
        private readonly ILogger<ChatService> _logger;
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;

        public ChatService(ILogger<ChatService> logger, IChatRepository chatRepo, IUserChatRepository userChatRepo, ITransactionProvider transactionProvider,IMediator mediator)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _transactionProvider = transactionProvider;
            _mediator = mediator;
        }
        public async Task UpdateGroupAvatarUrl(Guid chatId,string avatarUrl)
        {
        
            await _chatRepo.UpdateGroupAvatarUrl(chatId, avatarUrl);
            await _mediator.Publish(new GroupAvatarUpdatedNotification(chatId, avatarUrl));
        }

        public async Task<HashSet<Guid>> GetUsersInChatIdAsync(Guid chatId)
        {
            return await _userChatRepo.GetUsersInChatIdAsync(chatId);
        }
        public async Task<string> GetGroupAvatarUrlAsync(Guid chatId)
        {
             return await _chatRepo.GetGroupAvatarUrlAsync(chatId);
        }
        public async Task DeleteChatAsync(Guid chatId, Guid userId)
        {
                await _transactionProvider.ExecuteInTransactionAsync(async () =>
                { 
                await _userChatRepo.MarkChatAsDeletedAsync(chatId, userId);
                await _chatRepo.TryDeleteChatIfEmptyAsync(chatId);
                await _mediator.Publish(new ChatDeletedNotification(chatId, userId));
                });
        }

        public async Task<bool> IsChatExistingAsync(Guid chatId, Guid userId)
        {
            return await _chatRepo.CheckIfGroupExist(chatId, userId);
        }
    }
}
