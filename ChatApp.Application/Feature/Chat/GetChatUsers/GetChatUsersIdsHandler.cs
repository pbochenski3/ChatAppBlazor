using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatUsers
{
    public class GetChatUsersIdsHandler : IRequestHandler<GetChatUsersIdsQuery, HashSet<Guid>>
    {
        private readonly IUserChatRepository _userChatRepo;
        public GetChatUsersIdsHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<HashSet<Guid>> Handle(GetChatUsersIdsQuery r, CancellationToken cancellationToken)
        {
            return await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
        }
    }
}
