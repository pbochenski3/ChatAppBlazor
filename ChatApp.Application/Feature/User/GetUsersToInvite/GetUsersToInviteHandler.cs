using ChatApp.Application.DTO;
using ChatApp.Domain.Interfaces.Repository;
using Mapster;
using MediatR;

namespace ChatApp.Application.Feature.User.GetUsersToInvite
{
    public class GetUsersToInviteHandler : IRequestHandler<GetUsersToInviteQuery, List<UserSearchResultDTO>>
    {
        private readonly IUserRepository _userRepo;
        public GetUsersToInviteHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<List<UserSearchResultDTO>> Handle(GetUsersToInviteQuery r, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetAllUsersToInviteAsync(r.UserId, r.Query);
            return users.Adapt<List<UserSearchResultDTO>>();
        }
    }
}
