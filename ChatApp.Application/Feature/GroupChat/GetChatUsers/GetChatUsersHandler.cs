using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.GetChatUsers
{
    public class GetChatUsersHandler : IRequestHandler<GetChatUsersQuery, List<UserDTO>>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IUserRepository _userRepo;
        public GetChatUsersHandler(IUserChatRepository userChatRepo, IUserRepository userRepo)
        {
            _userChatRepo = userChatRepo;
            _userRepo = userRepo;
        }
        public async Task<List<UserDTO>> Handle(GetChatUsersQuery r, CancellationToken cancellationToken)
        {
            var userIds = await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
            var users = await _userRepo.GetUsersByIdsAsync(userIds);

            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl,
                IsOnline = u.IsOnline
            }).ToList();
        }
    }
}
