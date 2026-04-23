namespace ChatApp.Application.Feature.Chat.DeleteChat
{
    public record DeleteChatCommand(Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
