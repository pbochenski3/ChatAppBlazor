using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Invite.HandleInviteAction
{
    public class HandleInviteActionHandler : IRequestHandler<HandleInviteActionCommand, bool>
    {
        private readonly IInviteRepository _inviteRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _uow;
        public HandleInviteActionHandler(
            IInviteRepository inviteRepo,
            IContactRepository contactRepo,
            IMediator mediator,
            IChatRepository chatRepo,
            IUserChatRepository userChatRepo,
            IUnitOfWork uow
            )
        {
            _inviteRepo = inviteRepo;
            _contactRepo = contactRepo;
            _mediator = mediator;
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _uow = uow;
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
                await _contactRepo.AddContactAsync(contacts[0]);
                await _contactRepo.AddContactAsync(contacts[1]);
                var chat = await _chatRepo.GetChatAsync(contacts[0].UserID, contacts[1].UserID);
                if(chat == null)
                {
                var createdChat = Domain.Models.Chat.CreatePrivateChat(contacts);
                    await _chatRepo.AddChatAsync(createdChat);
                }
                else
                {
                    await _userChatRepo.SetChatAccessibilityAsync(chat.ChatID, true);
                }
            }
            await _uow.CommitAsync();
            await _mediator.Publish(new InviteActionNotification(invite.SenderID, invite.ReceiverID, r.Request.chatId, newChatId, r.Request.Response));
            return true;

        }
    }
}
