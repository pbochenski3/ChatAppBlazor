using MediatR;

namespace ChatApp.Web.Events
{
    public class SidebarEvents
    {
        public record ChatNameChanged(Guid ChatId, string NewName) : INotification;
        public record ChatListChanged() : INotification;
        public record InvitesListChanged() : INotification;
        public record SidebarLastMessageChanged(Guid ChatId, string LastSender, string LastMessage) : INotification;
        public record SidebarCounterUpdated(Guid ChatId,bool Status) : INotification;
    }
}
