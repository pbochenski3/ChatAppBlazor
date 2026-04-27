using ChatApp.Application.DTO;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Invite.GetUsersInvites
{
    public class GetUsersInvitesHandler : IRequestHandler<GetUserInvitesQuery, List<InviteDTO>>
    {
        private readonly IInviteRepository _inviteRepo;
        public GetUsersInvitesHandler(IInviteRepository inviteRepo)
        {
            _inviteRepo = inviteRepo;
        }
        public async Task<List<InviteDTO>> Handle(GetUserInvitesQuery r, CancellationToken cancellationToken)
        {
            var invites = await _inviteRepo.GetInvitesForUserAsync(r.UserId);
            return invites.Select(i => new InviteDTO
            {
                InviteID = i.InviteID,
                SenderID = i.SenderID,
                ReceiverID = i.ReceiverID,
                SenderUsername = i.Sender.Username,
                ReceiverUsername = i.Receiver.Username,
                SenderAvatarUrl = i.Sender.AvatarUrl,
                Status = i.Status,
            }).ToList();
        }
    }
}

