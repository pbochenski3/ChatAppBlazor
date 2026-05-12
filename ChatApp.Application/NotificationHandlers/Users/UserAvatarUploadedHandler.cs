using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Users
{
    public class UserAvatarUploadedHandler : INotificationHandler<UserAvatarUploadedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        private readonly IContactRepository _contactRepo;
        public UserAvatarUploadedHandler(ISignalRNotificationService signalR, IContactRepository contactRepo)
        {
            _signalR = signalR;
            _contactRepo = contactRepo;
        }
        public async Task Handle(UserAvatarUploadedNotification n, CancellationToken cancellationToken)
        {
            var contacts = await _contactRepo.GetAllContactsAsync(n.UserId);
            var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
            var allRecipients = contactsToNofitify.Append(n.UserId.ToString()).ToList();
            await _signalR.SendToUsers(allRecipients, "ContactAvatarReload", n.AvatarUrl, n.UserId);
        }
    }
}
