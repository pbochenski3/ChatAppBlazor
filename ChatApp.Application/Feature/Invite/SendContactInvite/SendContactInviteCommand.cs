namespace ChatApp.Application.Feature.Invite.SendContactInvite
{
    public record SendContactInviteCommand(Guid SenderId, Guid ReceiverId) : BaseCommand<bool>;
}
