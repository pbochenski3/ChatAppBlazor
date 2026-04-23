using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.Message;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatService _userChatService;
        private readonly IChatReadStatusService _readStatusService;
        private readonly IMediator _mediator;

        public MessageService(
            IMessageRepository messageRepo,
            IUserChatService userChatService,
            IChatReadStatusService readStatusService,
            IMediator mediator
            )
        {
            _messageRepo = messageRepo;
            _userChatService = userChatService;
            _readStatusService = readStatusService;
            _mediator = mediator;
        }

        public async Task SaveMessageAsync(MessageDTO messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Content) && string.IsNullOrWhiteSpace(messageDto.imageUrl))
            {
                throw new Exception("Message content cannot be empty");
            }

            var message = new Message
            {
                Content = messageDto.Content,
                imageUrl = messageDto.imageUrl,
                SenderID = messageDto.SenderID,
                ChatID = messageDto.ChatID,
                MessageID = messageDto.MessageID,
                SentAt = messageDto.SentAt,
                MessageType = messageDto.MessageType,

            };
                await _messageRepo.AddMessageAsync(message);
                await _readStatusService.SaveLastSentMessageIdAsync(messageDto.ChatID, messageDto.MessageID);
                await _mediator.Publish(new ChatMessageSendedNotification(messageDto));
        }

        public async Task<List<MessageDTO>> GetChatMessageHistoryAsync(Guid userId, Guid chatId, CancellationToken token)
        {
            bool isArchive = await _userChatService.IsChatArchivedAsync(chatId, userId);
            DateTime? cutoffDate = null;

            if (isArchive)
            {
                cutoffDate = await _readStatusService.GetLastMessageAtChatAsync(userId, chatId);
            }

            var messages = await _messageRepo.GetMessageHistoryAsync(userId, chatId, cutoffDate, token);

            if (token.IsCancellationRequested || messages == null || !messages.Any())
            {
                return new List<MessageDTO>();
            }

            return messages.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                imageUrl= m.imageUrl,
                SentAt = m.SentAt,
                SenderID = m.SenderID,
                SenderUsername = m.MessageType == MessageType.System ? "SYSTEM" : (m.Sender?.Username ?? "Unknown"),
                ChatID = m.ChatID,
                MessageType = m.MessageType,
            }).ToList();
        }
    }
}
