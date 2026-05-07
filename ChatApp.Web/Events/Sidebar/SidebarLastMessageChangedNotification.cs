using ChatApp.Application.DTO;
using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record SidebarLastMessageChangedNotification(MessageDTO Message) : INotification;

}
