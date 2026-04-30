using ChatApp.Web.Events;
using ChatApp.Web.Services.Actions.Interfaces;
using MediatR;

namespace ChatApp.Web.Handlers
{
    public class SidebarEventsHandler :
        INotificationHandler<SidebarEvents.ChatNameChanged>,
        INotificationHandler<SidebarEvents.ChatListChanged>,
        INotificationHandler<SidebarEvents.InvitesListChanged>,
        INotificationHandler<SidebarEvents.SidebarLastMessageChanged>,
        INotificationHandler<SidebarEvents.SidebarCounterUpdated>
    {
        private readonly ISidebarActionService _sidebarActionService;
        public SidebarEventsHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }
        public async Task Handle(SidebarEvents.ChatNameChanged notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleChatNameReloadAsync(notification.ChatId, notification.NewName,notification.UserId);
        }

        public async Task Handle(SidebarEvents.ChatListChanged notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleChatsLoadAsync();
        }

        public async Task Handle(SidebarEvents.InvitesListChanged notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleInvitesLoadAsync();
        }

        public async Task Handle(SidebarEvents.SidebarLastMessageChanged notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleSidebarLastMessageReloadAsync(notification.ChatId, notification.LastSender, notification.LastMessage);
        }

        public async Task Handle(SidebarEvents.SidebarCounterUpdated notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleCounterUpdateAsync(notification.ChatId, notification.Status);
        }

    }

}
