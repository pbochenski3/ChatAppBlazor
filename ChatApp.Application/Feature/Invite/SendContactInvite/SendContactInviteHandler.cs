using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Invite;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Invite.SendContactInvite
{
    public class SendContactInviteHandler : IRequestHandler<SendContactInviteCommand,bool>
    {
        private readonly IInviteRepository _inviteRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _uow;
        public SendContactInviteHandler(IInviteRepository inviteRepo,IMediator mediator,IUnitOfWork uow)
        {
            _inviteRepo = inviteRepo;
            _mediator = mediator;
            _uow = uow;
        }
        public async Task<bool> Handle(SendContactInviteCommand r, CancellationToken cancellationToken)
        {
            await _inviteRepo.AddInviteAsync(r.SenderId, r.ReceiverId);
            await _uow.CommitAsync();

            await _mediator.Publish(new ContactInviteSendedNotification(r.SenderId, r.ReceiverId));
            return true;
        }
    }
}
