using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Events;
using ChatApp.Web.Services.Actions;
using ChatApp.Web.Services.State;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;


public class ChatHubService : IAsyncDisposable
{
    public HubConnection? HubConnection { get; private set; }
    public event Func<string, Task>? InviteStatusMessage;
    public event Func<string, Guid, Task>? OnAvatarReload;
    public event Func<string, Guid, Task>? OnGroupAvatarReload;

    private readonly AppStateService _appStateService;
    private readonly string _baseHubUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatHubService> _logger;
    private readonly ChatActionService _chatActionService;
    private readonly SidebarActionService _sidebarActionService;
    private readonly ChatSettingsActionService _chatSettingsActionService;

    public ChatHubService(IConfiguration configuration,
        AppStateService appStateService,
        HttpClient httpClient,
        ChatActionService chatActionService,
        SidebarActionService sidebarActionService,
        ChatSettingsActionService chatSettingsActionService,
        ILogger<ChatHubService> logger)
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"] ?? throw new ArgumentNullException("SignalR:HubUrl");
        _httpClient = httpClient;
        _chatActionService = chatActionService;
        _sidebarActionService = sidebarActionService;
        _chatSettingsActionService = chatSettingsActionService;
        _logger = logger;
    }

    private void RegisterHandlers()
    {
        if (HubConnection == null) return;
        _appStateService.OnLogoutRequested += StopAsync;
        HubConnection.On<MessageDTO>("ReceiveMessage", (message) => _chatActionService.HandleIncomingMessageAsync(message));
        HubConnection.On("SidebarChatsReload",  async () => await _sidebarActionService.HandleChatsLoadAsync());
        HubConnection.On("SidebarInvitesReload", async () => await _sidebarActionService.HandleInvitesLoadAsync());
        //HubConnection.On<bool>("ContactInviteReload", async (_) => await TriggerReload(ReloadTarget.Global));
        HubConnection.On("ChatClose",  async () => await _chatActionService.HandleChatCloseAsync());
        HubConnection.On<Guid, bool>("ChatReload",  async (id, force) => await _chatActionService.HandleChatLoadAsync(new ContactSelectedArgs(id, force)));
        HubConnection.On<Guid>("UsersInChatReload", async (chatId) => await _chatActionService.HandleUserOnGroupLoadAsync(chatId));
        HubConnection.On<Guid, string>("UpdateChatName",  async (chatId, newName) => await _sidebarActionService.HandleChatNameReloadAsync(chatId, newName));
        HubConnection.On<Guid, string, string>("UpdateLastMessage",  async (chatId, lastSender, lastMessage) => await _sidebarActionService.HandleSidebarLastMessageReloadAsync(chatId, lastSender, lastMessage));
        HubConnection.On<string, Guid>("ContactAvatarReload", async (avatarUrl, userId) =>
        {
            await (OnAvatarReload?.Invoke(avatarUrl, userId) ?? Task.CompletedTask);
        });
        HubConnection.On<string, Guid>("GroupAvatarReload", async (avatarUrl, chatId) =>
        {
            await (OnGroupAvatarReload?.Invoke(avatarUrl, chatId) ?? Task.CompletedTask);
        });
        HubConnection.On<string>("ReceiveStatus", async (status) => await InviteStatusMessage.Invoke(status));
        HubConnection.On<Guid>("RequestLeaveGroupSignalR", async (chatId) =>
        {
            await HubConnection.InvokeAsync("RemoveMeFromChatGroup", chatId);
        });

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
        _appStateService.OnLogoutRequested -= StopAsync;
    }
}
