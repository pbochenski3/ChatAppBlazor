namespace ChatApp.Application.Feature.Contacts.DeleteContact
{
    public record DeleteContactCommand(Guid PrivateChatId, Guid UserId) : BaseCommand<bool>;
}
