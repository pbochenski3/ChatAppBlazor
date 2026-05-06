using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Sidebar
{
    public class InvitesListChangedHandler : INotificationHandler<InvitesListChangedNotification>
    {
        private readonly ISidebarActionService _sidebarActionService;

        public InvitesListChangedHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }

        public async Task Handle(InvitesListChangedNotification notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleInvitesLoadAsync();
        }
    }
}