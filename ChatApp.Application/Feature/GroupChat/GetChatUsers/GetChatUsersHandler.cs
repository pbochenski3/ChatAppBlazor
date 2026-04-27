using ChatApp.Application.DTO;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.GetChatUsers
{
    public class GetChatUsersHandler : IRequestHandler<GetChatUsersQuery, HashSet<UserDTO>>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IUserRepository _userRepo;
        public GetChatUsersHandler(IUserChatRepository userChatRepo, IUserRepository userRepo)
        {
            _userChatRepo = userChatRepo;
            _userRepo = userRepo;
        }
        public async Task<HashSet<UserDTO>> Handle(GetChatUsersQuery r, CancellationToken cancellationToken)
        {
            var userIds = await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
            var users = await _userRepo.GetUsersByIdsAsync(userIds);

            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl,
                IsOnline = u.IsOnline
            }).ToHashSet();
        }
    }
}
