using ChatApp.Application.DTO;
using ChatApp.ChatServer.Client.Services;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
public class ChatHubService : IAsyncDisposable
{
    private readonly string _hubUrl;
    public HubConnection HubConnection { get; private set; }
    public event Action<MessageDTO>? OnMessageReceived;
    public event Action<string>? RegisterStatusMessage;
    public event Action<string>? InviteStatusMessage;
    public event Func<bool,Task>? ContactInviteReload;
    public event Func<bool,Task>? InviteReload;
    public event Func<bool,Task>? ContactReload;
    public event Action<bool>? InviteReceived;
    public event Action<string,UserDTO>? LoginStatusMessage;
    private readonly AppStateService _appStateService;
    private readonly string? _baseHubUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatHubService> _logger;

    public ChatHubService(IConfiguration configuration, AppStateService appStateService,HttpClient httpClient, ILogger<ChatHubService> logger)
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"];
        _httpClient = httpClient;
        _httpClient = httpClient;
        _logger = logger;
    }
    private void RegisterHandlers()
    {
        if (HubConnection == null) return;
        HubConnection.On<MessageDTO>("ReceiveMessage", (message) =>
        {
            OnMessageReceived?.Invoke(message);
        });
        HubConnection.On<string>("ReceiveInviteStatus", (inviteStatusMessage) =>
        {
            InviteStatusMessage?.Invoke(inviteStatusMessage);
        });
        HubConnection.On<bool>("ContactInviteReload", (shouldReload) =>
        {
            ContactInviteReload?.Invoke(shouldReload);
        });
        HubConnection.On<bool>("InviteReload", (shouldReload) =>
        {
            InviteReload?.Invoke(shouldReload);
        });
        HubConnection.On<bool>("ContactReload", (shouldReload) =>
        {
            ContactReload?.Invoke(shouldReload);
        });
        HubConnection.On<bool>("InviteReceived", (shouldReload) =>
        {
            InviteReceived?.Invoke(shouldReload);
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
        if(HubConnection != null && HubConnection.State == HubConnectionState.Connected)
            return;
        HubConnection = new HubConnectionBuilder()
                .WithUrl(_baseHubUrl, options =>
                {
                    options.AccessTokenProvider = () =>
                    {
                        var token = _appStateService.CurrentUser?.Token;
                        return Task.FromResult(token);
                    };
                })
                .WithAutomaticReconnect()
                .Build();

                RegisterHandlers();
                await HubConnection.StartAsync();

    }
    public async Task LoginUserAsync(UserDTO dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/user/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var loggedInUser = await response.Content.ReadFromJsonAsync<UserDTO>();

            _appStateService.CurrentUser = loggedInUser;

            await StartAsync();

            LoginStatusMessage?.Invoke("Success", loggedInUser);
        }
        else
        {
            var errorCOntent = await response.Content.ReadAsStringAsync();
            throw new Exception($"{errorCOntent}");
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
        var response = await _httpClient.PostAsJsonAsync("api/user/register", dto);
        if (response.IsSuccessStatusCode)
        {
            RegisterStatusMessage?.Invoke("User registered successfully.");
        }
        else
        {
            var errorCOntent = await response.Content.ReadAsStringAsync();
            throw new Exception($"{errorCOntent}");
        }
    }
    public async Task<ChatDTO> GetChatInformation(Guid contactId)
    {
        return await HubConnection.InvokeAsync<ChatDTO>("GetChat", contactId);
    }

    public async Task<List<MessageDTO>> GetPrivateHistoryAsync(Guid contactId,Guid chatId)
    {
        try
        {
            return await HubConnection.InvokeAsync<List<MessageDTO>>("GetPrivateHistory", contactId, chatId);
        }
        catch (Exception)
        {
            _logger.LogInformation("Filed to load a chat messages");
            return new List<MessageDTO>();
        }
    }
    public Task<List<InviteDTO>> GetInvitesAsync()
    {
        return HubConnection.InvokeAsync<List<InviteDTO>>("GetInvites");
    }
    public async Task<List<ContactDTO>> GetContactsAsync()
    {
        return await HubConnection.InvokeAsync<List<ContactDTO>>("GetContacts");
    }
    public async Task<ContactDTO> GetCurrentContact(Guid contactId)
    {
        return await HubConnection.InvokeAsync<ContactDTO>("GetCurrentContacts", contactId);
    }
    public async Task<List<UserDTO>> GetUsersToInviteAsync(string query)
    {
        return await HubConnection.InvokeAsync<List<UserDTO>>("GetUsersToInvite", query);
    }
    public async Task SendInviteAsync(Guid receiverId)
    {
        await HubConnection.InvokeAsync("SendInvite",receiverId);
    }
    public async Task SendInviteActionAsync(Guid InviteId, bool Status)
    {
               await HubConnection.InvokeAsync("InviteAction", InviteId,Status);
    }
    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}