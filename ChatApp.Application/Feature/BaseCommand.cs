using ChatApp.Application.Common.Messaging;
using MediatR;

namespace ChatApp.Application.Feature
{
    public abstract record BaseCommand<TResponse> : ICommand<TResponse>, IHasDomainEvents
    {
        private readonly List<INotification> _events = new();
        public IReadOnlyList<INotification> DomainEvents => _events.AsReadOnly();

        public void AddEvent(INotification notification) => _events.Add(notification);
        public void ClearDomainEvents() => _events.Clear();
    }
}
