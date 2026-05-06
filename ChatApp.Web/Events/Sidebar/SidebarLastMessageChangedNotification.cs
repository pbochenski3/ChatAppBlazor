using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record SidebarLastMessageChangedNotification(Guid ChatId, string LastSender, string LastMessage) : INotification;

}
