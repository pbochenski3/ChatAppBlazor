namespace ChatApp.Application.Feature.Contact.DeleteContact
{
    public record DeleteContactCommand(Guid PrivateChatId, Guid UserId) : BaseCommand<bool>;
}
