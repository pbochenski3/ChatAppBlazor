using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record ChatListChangedNotification() : INotification;

}
