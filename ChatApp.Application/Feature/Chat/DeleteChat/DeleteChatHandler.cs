using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.DeleteChat
{
    public class DeleteChatHandler : IRequestHandler<DeleteChatCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;
        public DeleteChatHandler(IChatRepository chatRepo,IUserChatRepository userChatRepo, IMediator mediator)
        {
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(DeleteChatCommand r, CancellationToken cancellationToken)
        {
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive == true)
            {
                return false;
            }
            await _userChatRepo.MarkChatAsDeletedAsync(r.ChatId, r.UserId);
            await _chatRepo.TryDeleteChatIfEmptyAsync(r.ChatId);
            await _mediator.Publish(new ChatDeletedNotification(r.ChatId, r.UserId));
            return true;
        }
    }
}
