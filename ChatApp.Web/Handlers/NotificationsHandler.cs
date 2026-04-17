using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Common.Interfaces;
using MediatR;
using static ChatApp.Web.Events.ChatEvents;
using static ChatApp.Web.Events.SidebarEvents;

namespace ChatApp.Web.Handlers
{
    public class NotificationsHandler :
        INotificationHandler<ChatDeleted>,
        INotificationHandler<ChatDeletionFailed>,
        INotificationHandler<ChatLeft>,
        INotificationHandler<ChatLeavingFailed>,
        INotificationHandler<ContactDeleted>,
        INotificationHandler<ContactDeletionFailed>,
        INotificationHandler<LoadingFailed>,
        INotificationHandler<ChatNameChange>,
        INotificationHandler<ChangingChatNameFailed>,
        INotificationHandler<AddingUsersToGroupFailed>

    {
        private readonly INotificationService _notifcationService;
        public NotificationsHandler(INotificationService notificationService)
        {
            _notifcationService = notificationService;
        }
        public Task Handle(ChatDeleted notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
                $"Czat został usunięty!",
                NotificationType.Info);
            return Task.CompletedTask;
        }
        public Task Handle(ChatDeletionFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
                $"Usuwanie czatu się nie powiodło!",
                NotificationType.Error);
            return Task.CompletedTask;
        }

        public Task Handle(ChatLeft notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
                $"Czat został opuszczony!",
                NotificationType.Info);
            return Task.CompletedTask;
        }

        public Task Handle(ChatLeavingFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
                $"Opuszczanei czatu nie powiodło się!",
                NotificationType.Error);
            return Task.CompletedTask;
        }

        public Task Handle(ContactDeleted notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
               $"Kontakt został usunięty!",
               NotificationType.Info);
            return Task.CompletedTask;
        }

        public Task Handle(ContactDeletionFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
              $"Usuwanie kontaktu się nie powiodło!",
              NotificationType.Error);
            return Task.CompletedTask;
        }

        public Task Handle(LoadingFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
              $"Ładowanie się nie powiodło!",
              NotificationType.Error);
            return Task.CompletedTask;
        }

        public Task Handle(ChatNameChange notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
               $"Nazwa czatu zostałą zmieniona!",
               NotificationType.Info);
            return Task.CompletedTask;
        }

        public Task Handle(ChangingChatNameFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
             $"Zmiana nazwy czatu się nie powiodła!",
             NotificationType.Error);
            return Task.CompletedTask;
        }

        public Task Handle(AddingUsersToGroupFailed notification, CancellationToken cancellationToken)
        {
            _notifcationService.Notify(
             $"Dodawanie użytkowników się nie powiodło!",
             NotificationType.Error);
            return Task.CompletedTask;
        }
    }
}
