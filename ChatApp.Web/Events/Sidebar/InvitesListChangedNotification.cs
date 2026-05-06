using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record InvitesListChangedNotification() : INotification;

}
