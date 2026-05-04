using ChatApp.Application.Notifications.Message;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;

namespace ChatApp.Application.Feature.Messages.SendChatMessage
{
    public class SendChatMessageHandler : IRequestHandler<SendChatMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;
        public SendChatMessageHandler(IMediator mediator, IMessageRepository messageRepo, IUserChatRepository userChatRepo)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(SendChatMessageCommand r, CancellationToken cancellationToken)
        {
            var messageDto = r.Dto;
            await _messageRepo.AddMessageAsync(messageDto.Adapt<Message>());
            await _userChatRepo.UpdateLastSentMessageAsync(messageDto.ChatID, messageDto.MessageID);
            r.AddEvent(new ChatMessageSendedNotification(messageDto));
            return true;
        }

    }
}
