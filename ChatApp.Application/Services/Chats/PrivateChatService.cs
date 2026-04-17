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
        private readonly ITransactionProvider _transactionProvider;

        public PrivateChatService(ILogger<PrivateChatService> logger, IChatRepository chatRepo, IUserRepository userRepo, IUserChatRepository userChatRepo,IMessageRepository messageRepo, ITransactionProvider transactionProvider)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _userChatRepo = userChatRepo;
            _messageRepo = messageRepo;
            _transactionProvider = transactionProvider;
        }

        public async Task<Guid> CreatePrivateChatAsync(Guid userId1, Guid userId2)
        {
            return await _transactionProvider.ExecuteAsync(async () =>
            {
                var chat = await _chatRepo.GetChatAsync(userId1, userId2);

                if (chat == null)
                {
                    var user1 = await _userRepo.GetByIdAsync(userId1);
                    var user2 = await _userRepo.GetByIdAsync(userId2);

                    chat = new Chat
                    {
                        ChatID = Guid.CreateVersion7(),
                        CreatedAt = DateTime.UtcNow,
                        IsGroup = false,
                        UserChats = new List<UserChat>(),
                    };

                    chat.UserChats.Add(new UserChat
                    {
                        UserID = userId1,
                        ChatID = chat.ChatID,
                        ChatName = user2.Username,
                        IsArchive = false,
                    });

                    chat.UserChats.Add(new UserChat
                    {
                        UserID = userId2,
                        ChatID = chat.ChatID,
                        ChatName = user1.Username,
                        IsArchive = false,
                    });

                    await _chatRepo.AddChatAsync(chat);
                    var systemMessage = new Message
                    {
                        MessageID = Guid.CreateVersion7(),
                        ChatID = chat.ChatID,
                        Content = "Ten czat nie ma jeszcze wiadomości! Przywitaj się!",
                        SentAt = DateTime.UtcNow,
                        MessageType = MessageType.System
                    };
                    await _messageRepo.AddMessageAsync(systemMessage);
                }
                else
                {
                    await _userChatRepo.SetChatAccessibilityAsync(chat.ChatID, true);
                }
                return chat.ChatID;
            });

            }


        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            return await _userChatRepo.GetReceiverUserIdAsync(chatId, userId, token);
        }
    }
}
