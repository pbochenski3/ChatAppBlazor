using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateAdminFlagOnChat
{
    public class UpdateAdminFlagOnChatHandler : IRequestHandler<UpdateAdminFlagOnChatCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        public UpdateAdminFlagOnChatHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(UpdateAdminFlagOnChatCommand r, CancellationToken cancellationToken)
        {
            await _userChatRepo.UpdateAdminFlagAsync(r.UserId, r.ChatId, r.Flag);
            return true;
        }
    }
}
