using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat
{
    public class AddUsersToGroupHandler : IRequestHandler<AddUsersToGroupChatCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMediator _mediator;
        public AddUsersToGroupHandler(IChatRepository chatRepo, IUserRepository userRepo, IMessageRepository messageRepo, IMediator mediator, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _mediator = mediator;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(AddUsersToGroupChatCommand r, CancellationToken cancellationToken)
        {
            var isAdmin = await _userChatRepo.GetUserAdminFlagAsync(r.UserId, r.ChatId);
            if (!isAdmin)
            {
                r.AddEvent(new UserActionFailedNotification(r.UserId, "Nie posiadasz uprawnień!"));
                return false;
            }
            var chat = await _chatRepo.FetchChatById(r.ChatId);
            if (chat == null) return false;

            var admin = await _userRepo.GetByIdAsync(r.UserId);
            var usersToAdd = await _userRepo.GetUsersByIdsAsync(r.UsersToAdd);
            var systemMessage = chat.AddMembers(admin, usersToAdd);
            await _messageRepo.AddMessageAsync(systemMessage);
            r.AddEvent(new UsersAddedToGroupChatNotification(chat.ChatID, systemMessage, r.UsersToAdd));

            return true;
        }
    }
}
