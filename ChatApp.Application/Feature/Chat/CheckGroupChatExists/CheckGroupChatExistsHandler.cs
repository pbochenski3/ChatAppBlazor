using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.CheckGroupChatExists
{
    public class CheckGroupChatExistsHandler : IRequestHandler<CheckGroupChatExistsQuery, bool>
    {
        private readonly IChatRepository _chatRepo;
        public CheckGroupChatExistsHandler(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }
        public async Task<bool> Handle(CheckGroupChatExistsQuery r, CancellationToken cancellationToken)
        {
            return await _chatRepo.CheckIfGroupExist(r.ChatId, r.UserId);
        }
    }
}
