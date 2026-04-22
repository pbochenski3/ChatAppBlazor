using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatUsers
{
    public class GetChatUsersHandler : IRequestHandler<GetChatUsersQuery, HashSet<Guid>>
    {
        private readonly IUserChatRepository _userChatRepo;
        public GetChatUsersHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<HashSet<Guid>> Handle(GetChatUsersQuery r, CancellationToken cancellationToken)
        {
            return await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
        }
    }
}
