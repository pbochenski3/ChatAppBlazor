using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Repository;
using MediatR;

namespace ChatApp.Application.Feature.GroupChat.CreateGroupChat
{
    public class CreateGroupChatHandler : IRequestHandler<CreateGroupChatCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMediator _mediator;
        public CreateGroupChatHandler(IChatRepository chatRepo, IUserRepository userRepo, IMessageRepository messageRepo, IMediator mediator, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _mediator = mediator;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(CreateGroupChatCommand r, CancellationToken cancellationToken)
        {
            Domain.Models.Chat targetChat;
            Domain.Models.Message systemMessage;
            var existingsUsersIds = await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
            existingsUsersIds.UnionWith(r.UsersToAdd);
            var existingUsers = await _userRepo.GetUsersByIdsAsync(existingsUsersIds);
            var usersToAddList = r.UsersToAdd.ToList();
            var result = Domain.Models.Chat.CreateNewGroup(r.UserId, existingUsers);
            targetChat = result.Chat;
            systemMessage = result.SystemMessage;
            await _messageRepo.AddMessageAsync(systemMessage);
            await _chatRepo.AddChatAsync(targetChat);
            r.AddEvent(new GroupChatCreatedNotification(targetChat.ChatID, existingsUsersIds));
            return true;

        }
    }
}
