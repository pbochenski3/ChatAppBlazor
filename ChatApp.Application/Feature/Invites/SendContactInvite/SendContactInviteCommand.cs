namespace ChatApp.Application.Feature.Invites.SendContactInvite
{
    public record SendContactInviteCommand(Guid SenderId, Guid ReceiverId) : BaseCommand<bool>;
}
