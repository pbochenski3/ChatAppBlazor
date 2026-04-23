using ChatApp.Application.DTO;
using ChatApp.Web.Services.State;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using static ChatApp.Web.Events.ChatEvents;
using static ChatApp.Web.Events.SidebarEvents;


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
    private readonly IMediator _mediator;

    public ChatHubService(IConfiguration configuration,
        AppStateService appStateService,
        HttpClient httpClient,
        ILogger<ChatHubService> logger,
        IMediator mediator
        )
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"] ?? throw new ArgumentNullException("SignalR:HubUrl");
        _httpClient = httpClient;
        _logger = logger;
        _mediator = mediator;
    }

    private void RegisterHandlers()
    {
        if (HubConnection == null) return;
        _appStateService.OnLogoutRequested += StopAsync;
        HubConnection.On<MessageDTO>("ReceiveMessage", async (message) => await _mediator.Publish(new IncomingMessageReceived(message)));
        HubConnection.On("ChatClose", async () => await _mediator.Publish(new ChatRoomClosed()));
        HubConnection.On<Guid, bool>("ChatReload", async (id, force) => await _mediator.Publish(new ChatUpdated(id, force)));
        HubConnection.On<Guid>("UsersInChatReload", async (chatId) => await _mediator.Publish(new UsersInChatUpdated(chatId)));

        HubConnection.On<Guid, string>("UpdateChatName", async (chatId, newName) => await _mediator.Publish(new ChatNameChanged(chatId, newName)));
        HubConnection.On("SidebarChatsReload", async () => await _mediator.Publish(new ChatListChanged()));
        HubConnection.On("SidebarInvitesReload", async () => await _mediator.Publish(new InvitesListChanged()));
        HubConnection.On<Guid, string, string>("UpdateLastMessage", async (chatId, lastSender, lastMessage) => await _mediator.Publish(new SidebarLastMessageChanged(chatId, lastSender, lastMessage)));

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
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
        _appStateService.OnLogoutRequested -= StopAsync;
    }

}
