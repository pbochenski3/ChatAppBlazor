using Microsoft.AspNetCore.SignalR.Client;
public class ChatHubService : IAsyncDisposable
{
    private readonly string _hubUrl;
    public HubConnection HubConnection { get; private set; }
    public event Action<string, string>? OnMessageReceived;

    public ChatHubService(IConfiguration configuration)
    {
        _hubUrl = configuration["SignalR:HubUrl"];

        HubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        HubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            OnMessageReceived?.Invoke(user, message);
        });
    }

    public async Task StartAsync()
    {
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}