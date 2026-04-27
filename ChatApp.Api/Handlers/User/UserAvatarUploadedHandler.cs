using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.User
{
    public class UserAvatarUploadedHandler : INotificationHandler<UserAvatarUploadedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IContactRepository _contactRepo;
        public UserAvatarUploadedHandler(IHubContext<ChatHub> hubContext, IContactRepository contactRepo)
        {
            _hubContext = hubContext;
            _contactRepo = contactRepo;
        }
        public async Task Handle(UserAvatarUploadedNotification n, CancellationToken cancellationToken)
        {
            var contacts = await _contactRepo.GetAllContactsAsync(n.UserId);
            var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
            var allRecipients = contactsToNofitify.Append(n.UserId.ToString()).ToList();
            await _hubContext.Clients.Users(allRecipients).SendAsync("ContactAvatarReload", n.AvatarUrl, n.UserId);
        }
    }
}
