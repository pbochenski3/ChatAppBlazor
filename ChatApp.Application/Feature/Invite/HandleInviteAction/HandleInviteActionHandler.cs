using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Invite.HandleInviteAction
{
    public class HandleInviteActionHandler : IRequestHandler<HandleInviteActionCommand, bool>
    {
        private readonly IInviteRepository _inviteRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IUserRepository _userRepo;
        public HandleInviteActionHandler(
            IInviteRepository inviteRepo,
            IContactRepository contactRepo,
            IChatRepository chatRepo,
            IUserChatRepository userChatRepo,
            IUserRepository userRepo

            )
        {
            _inviteRepo = inviteRepo;
            _contactRepo = contactRepo;
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _userRepo = userRepo;
        }
        public async Task<bool> Handle(HandleInviteActionCommand r, CancellationToken cancellationToken)
        {
            var invite = await _inviteRepo.GetInviteByIdAsync(r.Request.InviteId);
            if (invite == null) return false;
            invite.Status = r.Request.Response;
            await _inviteRepo.UpdateInviteStatusAsync(invite);
            Guid newChatId = Guid.Empty;
            if (invite.Status == InviteStatus.Accepted)
            {
                var contacts = await Domain.Models.Invite.CreateContact(invite.SenderID, invite.ReceiverID);
                var chat = await _chatRepo.GetChatAsync(contacts[0].UserID, contacts[1].UserID);
                if (chat == null)
                {
                    await _contactRepo.AddContactAsync(contacts[0]);
                    await _contactRepo.AddContactAsync(contacts[1]);
                    var user1 = await _userRepo.GetByIdAsync(contacts[0].UserID);
                    var user2 = await _userRepo.GetByIdAsync(contacts[1].UserID);
                    var createdChat = Domain.Models.Chat.CreatePrivateChat(user1, user2);
                    newChatId = createdChat.ChatID;
                    await _chatRepo.AddChatAsync(createdChat);
                }
                else
                {
                    await _userChatRepo.SetChatAccessibilityAsync(chat.ChatID, true);
                    newChatId = chat.ChatID;
                }
            }
            r.AddEvent(new InviteActionNotification(invite.SenderID, invite.ReceiverID, r.Request.chatId, newChatId, r.Request.Response));
            return true;

        }
    }
}
