using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Chats.UpdateAdminFlagOnChat
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
            var isAdmin = await _userChatRepo.GetUserAdminFlagAsync(r.UserId, r.ChatId);
            if (!isAdmin)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Nie posiadasz uprawnień!"));
                return false;
            }
            await _userChatRepo.UpdateAdminFlagAsync(r.SelectedUserId, r.ChatId, r.Flag);
            return true;
        }
    }
}
