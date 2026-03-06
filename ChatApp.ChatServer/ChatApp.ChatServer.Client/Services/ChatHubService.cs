using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
public class ChatHubService : IAsyncDisposable
{
    private readonly string _hubUrl;
    public HubConnection HubConnection { get; private set; }
    public event Action<MessageDTO>? OnMessageReceived;
    public event Action<string>? RegisterStatusMessage;
    public event Action<string,UserDTO>? LoginStatusMessage;

    public ChatHubService(IConfiguration configuration)
    {
        _hubUrl = configuration["SignalR:HubUrl"];

        HubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        HubConnection.On<MessageDTO>("ReceiveMessage", (message) =>
        {
            OnMessageReceived?.Invoke(message);
        });
        HubConnection.On<string>("ReceiveRegistrationStatus", (statusMessage) =>
        {
            RegisterStatusMessage?.Invoke(statusMessage);
        });
        HubConnection.On<string,UserDTO>("ReceiveLoginStatus", (loginStatusMessage,user) =>
        {
            LoginStatusMessage?.Invoke(loginStatusMessage,user);
        });
    }
    public async Task SendMessageAsync(MessageDTO message)
    {
        if(message == null || string.IsNullOrWhiteSpace(message.Content))
        {
            throw new Exception("Message content cannot be empty.");
        }
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {

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
    public async Task LoginUserAsync(UserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new Exception("Username or password cannot be empty.");
        }
  
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("LoginUser", dto);
        }
        else
        {
            throw new InvalidOperationException("Connection Error");
        }
    }
    public async Task RegisterUserAsync(UserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
        {
            throw new Exception("Username must be at least 5 characters.");
        }
        else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            throw new Exception("Password must be at least 8 characters.");
        }
        else if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 20)
        {
            throw new Exception("Username must be less than 20 characters.");
        }
        else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length > 128)
        {
            throw new Exception("Password must be less than 128 characters.");
        }

        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("RegisterUser", dto);
        }
        else
        {
            throw new InvalidOperationException("Connection Error");
        }
    }
    public async Task<List<MessageDTO>> GetHistoryAsync(Guid contactid,int count)
    {
        return await HubConnection.InvokeAsync<List<MessageDTO>>("GetHistory",contactid,count);
    }

    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}