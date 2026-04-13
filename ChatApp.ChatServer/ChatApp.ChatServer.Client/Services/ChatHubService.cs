using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.ChatServer.Client.Services.State;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;


public class ChatHubService : IAsyncDisposable
{
    public HubConnection? HubConnection { get; private set; }
    public event Action<string>? RegisterStatusMessage;
    public event Action<string>? OnContactDelete;
    public event Action<string, UserDTO>? LoginStatusMessage;
    public event Func<MessageDTO, Task>? OnMessageReceived;
    public event Func<string, Guid, Task>? InviteStatusMessage;
    public event Func<ContactSelectedArgs, Task>? OnChatLoad;
    public event Func<ReloadTarget, Task>? OnAppReload;
    public event Func<Guid, Task>? OnUserInChatReload;
    public event Func<string,Guid, Task>? OnAvatarReload;
    public event Func<string,Guid, Task>? OnGroupAvatarReload;
    public event Func<Guid,string, Task>? OnChatNameChanged;
    public event Func<Guid,string,string, Task>? OnLastMessageChanged;

    private readonly AppStateService _appStateService;
    private readonly string _baseHubUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatHubService> _logger;

    public ChatHubService(IConfiguration configuration, AppStateService appStateService, HttpClient httpClient, ILogger<ChatHubService> logger)
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"] ?? throw new ArgumentNullException("SignalR:HubUrl");
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
            if (InviteStatusMessage != null)
            {
                await InviteStatusMessage.Invoke(status, contactId);
            }
        });

        HubConnection.On<bool>("SideBarReload", async (_) => await TriggerReload(ReloadTarget.Sidebar));
        HubConnection.On<bool>("InviteReload", async (_) => await TriggerReload(ReloadTarget.Invite));
        HubConnection.On<bool>("ContactInviteReload", async (_) => await TriggerReload(ReloadTarget.Global));
        HubConnection.On<bool>("ChatClose", async (_) => await TriggerReload(ReloadTarget.Chat));
        HubConnection.On<Guid, bool>("ChatReload", async (id, force) =>
        {
            if (OnChatLoad != null)
            {
                await OnChatLoad.Invoke(new ContactSelectedArgs(id, force));
            }
        });
        HubConnection.On<Guid>("UsersInChatReload", async (chatId) =>
        {
            if (OnUserInChatReload != null)
            {
                await OnUserInChatReload.Invoke(chatId);
            }
        });
        HubConnection.On<Guid, string>("UpdateChatName", async (chatId, newName) =>
        {
            await OnChatNameChanged.Invoke(chatId, newName);
        }); 
        HubConnection.On<Guid, string,string>("UpdateLastMessage", async (chatId, lastSender, lastMessage) =>
        {
            await OnLastMessageChanged.Invoke(chatId,lastSender,lastMessage);
        });
        HubConnection.On<string,Guid>("ContactAvatarReload", async (avatarUrl,userId) =>
        {
            await (OnAvatarReload?.Invoke(avatarUrl, userId) ?? Task.CompletedTask);
        });
        HubConnection.On<string, Guid>("GroupAvatarReload", async (avatarUrl, chatId) =>
        {
            await (OnGroupAvatarReload?.Invoke(avatarUrl, chatId) ?? Task.CompletedTask);
        });

    }
    private async Task TriggerReload(ReloadTarget target)
    {
        if (OnAppReload != null)
        {
            await OnAppReload.Invoke(target);
        }
    }
    public async Task JoinChatGroupSignalAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("JoinChatGroupSignalAsync", chatId);
    }
    public async Task StartAsync()
    {
        if (HubConnection != null && HubConnection.State == HubConnectionState.Connected)
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
    public async Task StopAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.StopAsync();
            await HubConnection.DisposeAsync();
            HubConnection = null;
        }
    }
    public async Task<string> GetUserAvatarUrlAsync()
    {
        return await HubConnection.InvokeAsync<string>("GetUserAvatarUrlAsync");
    }
    public async Task<List<InviteDTO>> GetUserInvitesAsync()
    {
        if (HubConnection == null) return new List<InviteDTO>();
        return await HubConnection.InvokeAsync<List<InviteDTO>>("GetUserInvitesAsync");
    }

    public async Task<ContactDTO?> GetContactByIdAsync(Guid contactId)
    {
        if (HubConnection == null) return null;
        return await HubConnection.InvokeAsync<ContactDTO?>("GetContactByIdAsync", contactId);
    }
    public async Task SendContactInviteAsync(Guid receiverId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("SendContactInviteAsync", receiverId);
    }
    public async Task HandleInviteActionAsync(Guid inviteId, bool status)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("HandleInviteActionAsync", inviteId, status);
    }
    public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
    {
        if (HubConnection == null) return new HashSet<UserDTO>();
        return await HubConnection.InvokeAsync<HashSet<UserDTO>>("GetChatUsersAsync", chatId);
    }
    public async Task<List<UserChatDTO>> GetUserChatListAsync()
    {
        if (HubConnection == null) return new List<UserChatDTO>();
        return await HubConnection.InvokeAsync<List<UserChatDTO>>("GetUserChatListAsync");
    }
    public async Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("CreateGroupChatAsync", chatId, userIdsToAdd);
    }
    public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
    {
        if (HubConnection == null) return new HashSet<Guid>();
        return await HubConnection.InvokeAsync<HashSet<Guid>>("GetChatUsersIdsAsync", chatId);
    }
    public async Task<bool> IsGroupChatExistingAsync(Guid chatId)
    {
        if (HubConnection == null) return false;
        return await HubConnection.InvokeAsync<bool>("IsGroupChatExistingAsync", chatId);
    }
    public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("AddUsersToGroupChatAsync", chatId, userIdsToAdd);
    }
    public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
    {
        if (HubConnection == null) return new List<UserChatDTO>();
        return await HubConnection.InvokeAsync<List<UserChatDTO>>("GetSidebarItemsAsync");
    }
    public async Task LeaveChatGroupAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("LeaveChatGroupAsync", chatId);
    }
    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}
