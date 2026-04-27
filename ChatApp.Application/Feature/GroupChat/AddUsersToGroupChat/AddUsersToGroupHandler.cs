using ChatApp.Application.Notifications.GroupChat;
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
            Domain.Models.Message systemMessage;
            var existingChat = await _chatRepo.FetchChatById(r.ChatId);
            var isArchive = existingChat.UserChats.Where(u => u.UserID == r.UserId).Any();
            var admin = await _userRepo.GetByIdAsync(r.UserId);
            var usersToAdd = await _userRepo.GetUsersByIdsAsync(r.UsersToAdd);
            if (isArchive)
            {
                systemMessage = existingChat.AddMembers(admin, usersToAdd);
                await _messageRepo.AddMessageAsync(systemMessage);
            }
            else
            {
                var ids = usersToAdd.Select(u => u.UserID).ToHashSet();
                await _userChatRepo.SetChatAccessibilityAsync(r.ChatId, true, ids);
                systemMessage = new Domain.Models.Message();
            }
            r.AddEvent(new UsersAddedToGroupChatNotification(existingChat.ChatID, systemMessage, r.UsersToAdd));
            return true;
        }
    }
}
