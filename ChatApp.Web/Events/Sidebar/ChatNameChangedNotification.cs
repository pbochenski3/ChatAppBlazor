using MediatR;

namespace ChatApp.Web.Events.Sidebar
{
    public record ChatNameChangedNotification(Guid ChatId, string NewName, Guid UserId) : INotification;

}
