using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record ChatUpdatedNotification(Guid ChatId, bool Force) : INotification;

}
