namespace ChatApp.Application.Feature.Messages.EditMessage
{
    public record EditMessageCommand(Guid ChatId, Guid MessageId, string Content, Guid UserId) : BaseCommand<bool>;
}
