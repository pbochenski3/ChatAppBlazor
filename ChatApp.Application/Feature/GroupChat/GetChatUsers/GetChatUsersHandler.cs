using ChatApp.Application.DTO;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.GetChatUsers
{
    public class GetChatUsersHandler : IRequestHandler<GetChatUsersQuery, HashSet<UserDTO>>
    {
        private readonly IUserChatRepository _userChatRepo;
        public GetChatUsersHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<HashSet<UserDTO>> Handle(GetChatUsersQuery r, CancellationToken cancellationToken)
        {
            var chatMembers = await _userChatRepo.GetChatMembersAsync(r.ChatId);
            return chatMembers.Adapt<HashSet<UserDTO>>();
        }
    }
}
