namespace ChatApp.Application.Feature.Chats.UpdateAdminFlagOnChat
{
    public record UpdateAdminFlagOnChatCommand(Guid ChatId, Guid UserId, Guid SelectedUserId, bool Flag) : BaseCommand<bool>;
}
