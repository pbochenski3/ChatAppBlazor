namespace ChatApp.ChatServer.Client.Services
{
    public class ChatStateService
    {
        public event Func<string, Task>? OnMessageRequested;
        public async Task RequestSendMessageAsync(string message)
        {
            if (OnMessageRequested != null)
            {
                await OnMessageRequested.Invoke(message);
            }
        }
    }
}
