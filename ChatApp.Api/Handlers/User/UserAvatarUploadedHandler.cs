using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.User
{
    public class UserAvatarUploadedHandler : INotificationHandler<UserAvatarUploadedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IContactService _contactService;
        public UserAvatarUploadedHandler(IHubContext<ChatHub> hubContext, IContactService contactService)
        {
            _hubContext = hubContext;
            _contactService = contactService;
        }
        public async Task Handle(UserAvatarUploadedNotification n, CancellationToken cancellationToken)
        {
            var contacts = await _contactService.GetAllContactAsync(n.UserId);
            var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
            var allRecipients = contactsToNofitify.Append(n.UserId.ToString()).ToList();
            await _hubContext.Clients.Users(allRecipients).SendAsync("ContactAvatarReload", n.AvatarUrl, n.UserId);
        }
    }
}
