namespace ChatApp.Application.Feature.Messages.DeleteMessage
{
    public record DeleteMessageCommand(Guid MessageId, Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
