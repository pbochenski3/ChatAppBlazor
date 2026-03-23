using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.ChatServer.Client.Services;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
public class ChatHubService : IAsyncDisposable
{
    public HubConnection HubConnection { get; private set; }
    public event Func<MessageDTO, Task>? OnMessageReceived;
    public event Action<string>? RegisterStatusMessage;
    public event Action<string>? OnContactDelete;
    public event Func<string, Guid, Task> InviteStatusMessage;
    public event Func<Guid, Task> OnChatLoad;
    public event Func<ReloadTarget, Task>? OnAppReload;
    public event Action<string, UserDTO>? LoginStatusMessage;
    private readonly AppStateService _appStateService;
    private readonly string? _baseHubUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatHubService> _logger;

    public ChatHubService(IConfiguration configuration, AppStateService appStateService, HttpClient httpClient, ILogger<ChatHubService> logger)
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"]!;
        _httpClient = httpClient;
        _httpClient = httpClient;
        _logger = logger;
    }
    private void RegisterHandlers()
    {
        if (HubConnection == null) return;
        HubConnection.On<MessageDTO>("ReceiveMessage", async (message) =>
        {
            if (OnMessageReceived != null)
            {
                await OnMessageReceived.Invoke(message);
            }
        });
        HubConnection.On<string, Guid>("ReceiveStatus", async (status, contactId) =>
            {
                var handler = InviteStatusMessage;
                if (handler != null) await handler.Invoke(status, contactId);
            });
        HubConnection.On<bool>("SideBarReload", async (_) => await TriggerReload(ReloadTarget.Sidebar));
        HubConnection.On<bool>("InviteReload", async (_) => await TriggerReload(ReloadTarget.Invite));
        HubConnection.On<bool>("ContactInviteReload", async (_) => await TriggerReload(ReloadTarget.Global));
        HubConnection.On<Guid>("ChatReload", async (id) =>
        {
            var handler = OnChatLoad;
            if (handler != null) await handler.Invoke(id);
        });

    }
    private async Task TriggerReload(ReloadTarget target)
    {
        var handler = OnAppReload;
        if (handler != null) await handler.Invoke(target);
    }
    public async Task MarkMessageAsReaded(Guid chatId, Guid messageId)
    {
        await HubConnection.InvokeAsync("MarkMessage", chatId, messageId);
    }
    public async Task MarkChatMessagesAsReaded(Guid chatId, CancellationToken token)
    {
        await HubConnection.InvokeAsync("MarkChatMessage", chatId, token);
    }
    public async Task<int> GetUnreadMessagesCounter(Guid chatId)
    {
        return await HubConnection.InvokeAsync<int>("FetchUnreadCount", chatId);
    }
    public async Task SendMessageAsync(MessageDTO message, Guid chatId)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Content))
        {
            throw new Exception("Message content cannot be empty.");
        }
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {

            await HubConnection.InvokeAsync("SendMessage", message, chatId);
        }
    }
    public async Task JoinGroupSignal(Guid chatId)
    {
        await HubConnection.InvokeAsync("JoinGroupSignal", chatId);
    }
    public async Task StartAsync()
    {
        if (HubConnection != null && HubConnection.State == HubConnectionState.Connected)
            return;
        HubConnection = new HubConnectionBuilder()
                .WithUrl(_baseHubUrl!, options =>
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
            if (loggedInUser != null)
            {
                await StartAsync();
                LoginStatusMessage?.Invoke("Success", loggedInUser);
            }
            else
            {
                throw new Exception($"LoggedUser is null");
            }
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
    public async Task<UserChatDTO> GetChatInformation(Guid contactId, CancellationToken token)
    {
        return await HubConnection.InvokeAsync<UserChatDTO>("GetChat", contactId, token);
    }
    public async Task ChatRestore(Guid contactId)
    {
        await HubConnection.InvokeAsync("ChatRestore");
    }
    public async Task<List<MessageDTO>> GetPrivateHistory(Guid chatId, CancellationToken token)
    {
        try
        {
            return await HubConnection.InvokeAsync<List<MessageDTO>>("GetPrivateHistory", chatId, token);
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
        await HubConnection.InvokeAsync("SendInvite", receiverId);
    }
    public async Task<bool> GetChatStatus(Guid chatId, Guid ContactId)
    {
        return await HubConnection.InvokeAsync<bool>("GetChatStatus", chatId, ContactId);
    }
    public async Task SendInviteActionAsync(Guid InviteId, bool Status)
    {
        await HubConnection.InvokeAsync("InviteAction", InviteId, Status);
    }
    public async Task DeleteContactAsync(Guid chatId)
    {
        await HubConnection.InvokeAsync("DeleteContact", chatId);
    }
    public async Task<List<ContactDTO>> GetContactList()
    {
        return await HubConnection.InvokeAsync<List<ContactDTO>>("GetContactList");

    }
    public async Task<HashSet<UserDTO>> GetGroupUsers(Guid chatId)
    {
        return await HubConnection.InvokeAsync<HashSet<UserDTO>>("GetUsersFromGroup", chatId);
    }
    public async Task<List<ChatDTO>> GetChatList()
    {
        return await HubConnection.InvokeAsync<List<ChatDTO>>("GetChatList");
    }
    public async Task CreateChatGroup(Guid chatId,HashSet<Guid> contactId)
    {
        await HubConnection.InvokeAsync("CreateGroupChat", chatId,contactId);
    }
    public async Task<HashSet<Guid>> LoadUserInChatList(Guid chatId)
    {
        return await HubConnection.InvokeAsync<HashSet<Guid>>("GetUsersInChat)", chatId);
    }
    public async Task<bool> CheckIfGroupExist(Guid chatId)
    {
        return await HubConnection.InvokeAsync<bool>("CheckIfGroupExist", chatId);
    }
    public async Task AddUserToGroup(Guid chatId,HashSet<Guid> usersToAdd)
    {;
        await HubConnection.InvokeAsync("AddUsersToGroup", chatId, usersToAdd);
    }
   
    public async Task<List<UserChatDTO>> LoadSidebarItems()
    {
        return await HubConnection.InvokeAsync<List<UserChatDTO>>("GetSidebarList");
    }
    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}