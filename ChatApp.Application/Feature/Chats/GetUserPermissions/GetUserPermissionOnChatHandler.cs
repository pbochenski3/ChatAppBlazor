using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Chats.GetUserPermissions
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
