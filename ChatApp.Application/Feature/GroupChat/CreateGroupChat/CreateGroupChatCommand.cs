namespace ChatApp.Application.Feature.GroupChat.CreateGroupChat
{
    public record CreateGroupChatCommand(Guid ChatId, HashSet<Guid> UsersToAdd, Guid UserId) : BaseCommand<bool>;


}
