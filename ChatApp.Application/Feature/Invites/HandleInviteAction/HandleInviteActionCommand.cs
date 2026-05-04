using ChatApp.Application.DTO.Requests;

namespace ChatApp.Application.Feature.Invites.HandleInviteAction
{
    public record HandleInviteActionCommand(InviteActionRequest Request) : BaseCommand<bool>;
}
