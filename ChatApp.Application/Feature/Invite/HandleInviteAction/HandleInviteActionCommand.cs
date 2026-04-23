using ChatApp.Application.DTO.Requests;

namespace ChatApp.Application.Feature.Invite.HandleInviteAction
{
    public record HandleInviteActionCommand(InviteActionRequest Request) : BaseCommand<bool>;
}
