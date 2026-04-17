using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;

namespace ChatApp.Web.Services.Common.Interfaces
{
    public interface INotificationService
    {
        event Action<NotificationMessage> OnShow;
        void Notify(string message, NotificationType type = NotificationType.Info, string title = "");
    }
}
