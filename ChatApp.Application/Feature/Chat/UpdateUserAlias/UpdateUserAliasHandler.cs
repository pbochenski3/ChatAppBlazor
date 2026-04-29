using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateUserAlias
{
    public class UpdateUserAliasHandler : IRequestHandler<UpdateUserAliasCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        public UpdateUserAliasHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(UpdateUserAliasCommand r, CancellationToken cancellationToken)
        {
            var request = r.request;
            await _userChatRepo.UpdateAliasOnChat(request.changeUserId,r.ChatId, request.Alias);
   
            r.AddEvent(new ChatUserAliasChangedNotification(r.ChatId,r.request));
            return true;
        }
    }
}
