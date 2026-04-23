//using ChatApp.Application.Notifications.User;
//using MediatR;
//using Microsoft.AspNetCore.SignalR;

//namespace ChatApp.Api.Handlers.User
//{
//    public class UserAvatarUploadedHandler : INotificationHandler<UserAvatarUploadedNotification>
//    {
//        private readonly IHubContext<ChatHub> _hubContext;
//        public UserAvatarUploadedHandler(IHubContext<ChatHub> hubContext)
//        {
//            _hubContext = hubContext;
//        }
//        public async Task Handle(UserAvatarUploadedNotification n, CancellationToken cancellationToken)
//        {
//            var contacts = await _contactService.GetAllContactAsync(n.UserId);
//            var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
//            var allRecipients = contactsToNofitify.Append(n.UserId.ToString()).ToList();
//            await _hubContext.Clients.Users(allRecipients).SendAsync("ContactAvatarReload", n.AvatarUrl, n.UserId);
//        }
//    }
//}
