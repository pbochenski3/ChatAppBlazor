namespace ChatApp.Application.Interfaces
{
    public interface ISignalRNotificationService
    {
        public Task SendToUser(string user, string methodName, params object[] args);
        public Task SendToUsers(List<string> users, string methodName, params object[] args);
        public Task SendToGroup(string group, string methodName, params object[] args);
    }
}
