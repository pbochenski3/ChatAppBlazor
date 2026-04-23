namespace ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat
{
    public record AddUsersToGroupChatCommand(Guid ChatId, HashSet<Guid> UsersToAdd, Guid UserId) : BaseCommand<bool>;
}
