namespace ChatApp.Application.Feature.Chats.DeleteChat
{
    public record DeleteChatCommand(Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
