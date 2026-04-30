using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetUserPermissions
{
    public class GetUserPermissionOnChatHandler : IRequestHandler<GetUserPermissionsOnChatQuery, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        public GetUserPermissionOnChatHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(GetUserPermissionsOnChatQuery r, CancellationToken cancellationToken)
        {
           return await _userChatRepo.GetUserAdminFlagAsync(r.UserId, r.ChatId);
        }
    }
}
