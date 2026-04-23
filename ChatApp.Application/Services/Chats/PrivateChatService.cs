using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ChatApp.Application.Services.Chats
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly ILogger<PrivateChatService> _logger;
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;

        public PrivateChatService(ILogger<PrivateChatService> logger, IChatRepository chatRepo, IUserRepository userRepo, IUserChatRepository userChatRepo,IMessageRepository messageRepo)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;
        }

        public async Task<Guid> CreatePrivateChatAsync(Guid userId1, Guid userId2)
        {
                var chat = await _chatRepo.GetChatAsync(userId1, userId2);

                if (chat == null)
                {
                    var user1 = await _userRepo.GetByIdAsync(userId1);
                    var user2 = await _userRepo.GetByIdAsync(userId2);

                    var result = Chat.CreatePrivateChat(user1,user2);
                    await _chatRepo.AddChatAsync(result);
                }
                else
                {
                    await _userChatRepo.SetChatAccessibilityAsync(chat.ChatID, true);
                }
                return chat.ChatID;

            }


        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            return await _userChatRepo.GetReceiverUserIdAsync(chatId, userId, token);
        }
    }
}
