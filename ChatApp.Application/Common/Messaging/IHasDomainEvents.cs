using MediatR;

namespace ChatApp.Application.Common.Messaging
{
    public interface IHasDomainEvents
    {
        IReadOnlyList<INotification> DomainEvents { get; }
        void AddEvent(INotification notification);
        void ClearDomainEvents();
    }
}
