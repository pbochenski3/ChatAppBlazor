using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Invite.SendContactInvite
{
    public class SendContactInviteHandler : IRequestHandler<SendContactInviteCommand, bool>
    {
        private readonly IInviteRepository _inviteRepo;
        private readonly IMediator _mediator;
        public SendContactInviteHandler(IInviteRepository inviteRepo, IMediator mediator)
        {
            _inviteRepo = inviteRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(SendContactInviteCommand r, CancellationToken cancellationToken)
        {
            await _inviteRepo.AddInviteAsync(r.SenderId, r.ReceiverId);

            r.AddEvent(new ContactInviteSendedNotification(r.SenderId, r.ReceiverId));
            return true;
        }
    }
}
