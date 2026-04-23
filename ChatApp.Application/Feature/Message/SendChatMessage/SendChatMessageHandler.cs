using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Message;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Message.SendChatMessage
{
    public class SendChatMessageHandler : IRequestHandler<SendChatMessageCommand,bool>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;
        public SendChatMessageHandler(IMediator mediator,IMessageRepository messageRepo, IUserChatRepository userChatRepo)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(SendChatMessageCommand r, CancellationToken cancellationToken)
        {
            var messageDto = r.Dto;

            var message = new Domain.Models.Message
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
                await _userChatRepo.UpdateLastSentMessageAsync(messageDto.ChatID, messageDto.MessageID);
                r.AddEvent(new ChatMessageSendedNotification(messageDto));
            return true;
        }

    }
}
