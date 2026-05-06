namespace ChatApp.Application.Feature.GroupChat.DeleteChat
{
    public record DeleteChatCommand(Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
