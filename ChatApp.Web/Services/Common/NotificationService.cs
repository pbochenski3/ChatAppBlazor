using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Web.Services.Common.Interfaces;

namespace ChatApp.Web.Services.Common
{
    public class NotificationService : INotificationService
    {
        public event Action<NotificationMessage>? OnShow;
        public void Notify(string message, NotificationType type = NotificationType.Info, string title = "")
        {
            var notification = new NotificationMessage
            {
                Message = message,
                Type =  type,
                Title = title
            };

            OnShow?.Invoke(notification);
        }
    }
}
