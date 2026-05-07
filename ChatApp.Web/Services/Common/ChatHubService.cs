using ChatApp.Application.DTO;
using ChatApp.Web.Events.Chat;
using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Common;
using ChatApp.Web.Services.State;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;


public class ChatHubService : IAsyncDisposable
{
    public HubConnection? HubConnection { get; private set; }
    public event Func<string, Guid, Task>? OnAvatarReload;
    public event Func<string, Guid, Task>? OnGroupAvatarReload;

    private readonly AppStateService _appStateService;
    private readonly string _baseHubUrl;
    private readonly ILogger<ChatHubService> _logger;
    private readonly IMediator _mediator;
    private readonly INotificationService _notify;

    public ChatHubService(
        IConfiguration configuration,
        AppStateService appStateService,
        ILogger<ChatHubService> logger,
        IMediator mediator,
        INotificationService notify
        )
    {
        _appStateService = appStateService;
        _baseHubUrl = configuration["SignalR:HubUrl"] ?? throw new ArgumentNullException("SignalR:HubUrl");
        _logger = logger;
        _mediator = mediator;
        _notify = notify;
    }

    private void RegisterHandlers()
    {
        if (HubConnection == null) return;
        _appStateService.OnLogoutRequested += StopAsync;
        HubConnection.On<MessageDTO>("ReceiveMessage", async (message) => await _mediator.Publish(new IncomingMessageReceivedNotification(message)));
        HubConnection.On("ChatClose", async () => await _mediator.Publish(new ChatRoomClosedNotification()));
        HubConnection.On<Guid, bool>("ChatReload", async (id, force) => await _mediator.Publish(new ChatUpdatedNotification(id, force)));
        HubConnection.On<Guid>("UsersInChatReload", async (chatId) => await _mediator.Publish(new UsersInChatUpdatedNotification(chatId)));
        HubConnection.On<Guid, Guid, string>("UserAliasChanged", async (chatId, userId, newAlias) => await _mediator.Publish(new UserAliasChangedNotification(chatId, userId, newAlias)));

        HubConnection.On<Guid, string, Guid>("UpdateChatName", async (chatId, newName, userId) => await _mediator.Publish(new ChatNameChangedNotification(chatId, newName, userId)));
        HubConnection.On("SidebarChatsReload", async () => await _mediator.Publish(new ChatListChangedNotification()));
        HubConnection.On("SidebarInvitesReload", async () => await _mediator.Publish(new InvitesListChangedNotification()));
        HubConnection.On<MessageDTO>("UpdateLastMessage", async (message) => await _mediator.Publish(new SidebarLastMessageChangedNotification(message)));
        HubConnection.On<Guid, Guid, bool>("UpdateFlagOnChat", async (userId, chatId, flag) => await _mediator.Publish(new RequestToUpdateFlagOnChatNotification(userId, chatId, flag)));

        HubConnection.On<string, Guid>("ContactAvatarReload", async (avatarUrl, userId) =>
        {
            await (OnAvatarReload?.Invoke(avatarUrl, userId) ?? Task.CompletedTask);
        });
        HubConnection.On<string, Guid>("GroupAvatarReload", async (avatarUrl, chatId) =>
        {
            await (OnGroupAvatarReload?.Invoke(avatarUrl, chatId) ?? Task.CompletedTask);
        });
        HubConnection.On<string>("NotifyError", (status) => _notify.Notify(status, ChatApp.Domain.Enums.NotificationType.Warning));
        HubConnection.On<string>("NotifyInfo", (status) => _notify.Notify(status, ChatApp.Domain.Enums.NotificationType.Info));
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
