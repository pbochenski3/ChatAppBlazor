namespace ChatApp.Application.Feature.GroupChat.LeaveGroupChatAsync
{
    public record LeaveGroupChatCommand(Guid ChatId, Guid UserId, string Username) : BaseCommand<bool>;
}
