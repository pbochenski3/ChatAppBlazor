using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateChatName
{
    public class UpdateChatNameHandler : IRequestHandler<UpdateChatNameCommand,bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;
        public UpdateChatNameHandler(IChatRepository chatRepo,IUserChatRepository userChatRepo,IMediator mediator)
        { 
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(UpdateChatNameCommand r, CancellationToken cancellationToken)
        {
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive)
            {
                return false;
            }
            var isGroup = await _chatRepo.IsChatGroupAsync(r.ChatId);
            if (isGroup == true)
            {
                await _chatRepo.UpdateChatNameAsync(r.ChatId, r.Request.NewName);
            }
            else
            {
                await _userChatRepo.SetNewChatNameAsync(r.ChatId, r.UserId, r.Request.NewName);
            }

            await _mediator.Publish(new ChatNameUpdatedNotification(r.ChatId, r.Request));
            return true;
        }
    }
}
