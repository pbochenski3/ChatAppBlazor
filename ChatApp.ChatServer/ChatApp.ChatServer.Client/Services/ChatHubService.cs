using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.ChatServer.Client.Services.Actions;
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
    public event Func<string, Task>? InviteStatusMessage;
    public event Func<ContactSelectedArgs, Task>? OnChatLoad;
    public event Func<ReloadTarget, Task>? OnAppReload;
    public event Func<Guid, Task>? OnUserInChatReload;
    public event Func<string, Guid, Task>? OnAvatarReload;
    public event Func<string, Guid, Task>? OnGroupAvatarReload;
    public event Func<Guid, string, Task>? OnChatNameChanged;
    public event Func<Guid, string, string, Task>? OnLastMessageChanged;

    private readonly AppStateService _appStateService;
    private readonly string _baseHubUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatHubService> _logger;
    private readonly ChatActionService _chatActionService;

    public ChatHubService(IConfiguration configuration,
        AppStateService appStateService,
        HttpClient httpClient,
        ChatActionService chatActionService,
        ILogger<ChatHubService> logger)
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"] ?? throw new ArgumentNullException("SignalR:HubUrl");
        _httpClient = httpClient;
        _chatActionService = chatActionService;
        _logger = logger;
    }

    private void RegisterHandlers()
    {
        if (HubConnection == null) return;

        HubConnection.On<MessageDTO>("ReceiveMessage", async (message) =>
        {
                await _chatActionService.HandleIncomingMessageAsync(message);
        });

        HubConnection.On<string>("ReceiveStatus", async (status) =>
        {
            if (InviteStatusMessage != null)
            {
                await InviteStatusMessage.Invoke(status);
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
        HubConnection.On<Guid, string, string>("UpdateLastMessage", async (chatId, lastSender, lastMessage) =>
        {
            await OnLastMessageChanged.Invoke(chatId, lastSender, lastMessage);
        });
        HubConnection.On<string, Guid>("ContactAvatarReload", async (avatarUrl, userId) =>
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
        SubscribeToEvent();
        RegisterHandlers();
        await HubConnection.StartAsync();
    }
    public void SubscribeToEvent()
    {
        _chatActionService.OnJoinGroupRequested += JoinChatGroupSignalAsync;
    }
    public void UnsubscribeFromEvent()
    {
        _chatActionService.OnJoinGroupRequested -= JoinChatGroupSignalAsync;
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
    //public async Task<List<UserChatDTO>> GetUserChatListAsync()
    //{
    //    if (HubConnection == null) return new List<UserChatDTO>();
    //    return await HubConnection.InvokeAsync<List<UserChatDTO>>("GetUserChatListAsync");
    //}
    public async Task JoinChatGroupSignalAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("JoinChatGroupSignalAsync", chatId);
    }
    public async Task<string> GetUserAvatarUrlAsync()
    {
        return await HubConnection.InvokeAsync<string>("GetUserAvatarUrlAsync");
    }
    public async ValueTask DisposeAsync()
    {
        if(_chatActionService != null)
        {
            _chatActionService.OnJoinGroupRequested -= JoinChatGroupSignalAsync;
        }    
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}
