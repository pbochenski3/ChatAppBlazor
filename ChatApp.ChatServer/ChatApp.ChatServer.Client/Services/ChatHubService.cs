using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
public class ChatHubService : IAsyncDisposable
{
    private readonly string _hubUrl;
    public HubConnection HubConnection { get; private set; }
    public event Action<string, string>? OnMessageReceived;
    public event Action<string>? RegisterStatusMessage;

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
        HubConnection.On<string>("ReceiveRegistrationStatus", (statusMessage) =>
        {
            RegisterStatusMessage?.Invoke(statusMessage);
        });
    }
    public async Task SendMessageAsync(MessageDTO message)
    {
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            message.SentAt = DateTime.Now;

            await HubConnection.InvokeAsync("SendMessage", message);
        }
    }
    public async Task StartAsync()
    {
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }
    public async Task RegisterUserAsync(UserDTO dto)
    {
        //if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
        //{
        //    throw new Exception("Username must be at least 5 characters.");
        //}
        //else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        //{
        //    throw new Exception("Password must be at least 8 characters.");
        //}
        //else if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 20)
        //{
        //    throw new Exception("Username must be less than 20 characters.");
        //}
        //else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length > 128)
        //{
        //    throw new Exception("Password must be less than 128 characters.");
        //}

            if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("SendUser", dto);
        }
        else
        {
            throw new InvalidOperationException("Connection Error");
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