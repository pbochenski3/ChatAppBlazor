using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record SidebarCounterUpdatedNotification(Guid ChatId, bool Status) : INotification;

}
