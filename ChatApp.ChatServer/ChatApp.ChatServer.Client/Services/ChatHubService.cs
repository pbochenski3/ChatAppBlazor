using ChatApp.Application.DTO;
using ChatApp.ChatServer.Client.Services;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

public class ChatHubService : IAsyncDisposable
{
    public HubConnection? HubConnection { get; private set; }
    public event Func<MessageDTO, Task>? OnMessageReceived;
    public event Action<string>? RegisterStatusMessage;
    public event Action<string>? OnContactDelete;
    public event Func<string, Guid, Task>? InviteStatusMessage;
    public event Func<ContactSelectedArgs, Task>? OnChatLoad;
    public event Func<ReloadTarget, Task>? OnAppReload;
    public event Func<Guid, Task>? OnUserInChatReload;
    public event Func<string,Guid, Task>? OnAvatarReload;
    public event Func<Guid,string, Task>? OnChatNameChanged;
    public event Func<Guid,string,string, Task>? OnLastMessageChanged;
    public event Action<string, UserDTO>? LoginStatusMessage;

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
        HubConnection.On<string,Guid>("AvatarReload", async (avatarUrl,changignUserId) =>
        {
            if (OnAvatarReload != null)
            {
                await OnAvatarReload.Invoke(avatarUrl, changignUserId);
            }
        });


    }
    private async Task TriggerReload(ReloadTarget target)
    {
        if (OnAppReload != null)
        {
            await OnAppReload.Invoke(target);
        }
    }
    public async Task MarkMessageAsReadAsync(Guid chatId, Guid messageId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("MarkMessageAsReadAsync", chatId, messageId);
    }
    public async Task MarkChatMessagesAsReadAsync(Guid chatId, CancellationToken token)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("MarkChatMessagesAsReadAsync", chatId, token);
    }
    public async Task<int> GetUnreadMessageCountAsync(Guid chatId)
    {
        if (HubConnection == null) return 0;
        return await HubConnection.InvokeAsync<int>("GetUnreadMessageCountAsync", chatId);
    }
    public async Task SendMessageAsync(MessageDTO message, Guid chatId)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Content))
        {
            throw new ArgumentException("Message content cannot be empty.");
        }
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("SendMessageAsync", message, chatId);
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
    public async Task UpdateUserAvatarAsync(MultipartFormDataContent file, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync("api/user/updateAvatar", file);
        if (response.IsSuccessStatusCode)
        {

            HubConnection?.InvokeAsync("UpdateUserAvatarAsync");

        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }


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
                throw new Exception("Logged user is null");
            }
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }
    }
    public async Task RegisterUserAsync(UserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
        {
            throw new ArgumentException("Username must be at least 5 characters.");
        }
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters.");
        }
        if(dto.Username.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Username is already oucppied.");
        };
        
        var response = await _httpClient.PostAsJsonAsync("api/user/register", dto);
        if (response.IsSuccessStatusCode)
        {
            RegisterStatusMessage?.Invoke("User registered successfully.");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }
    }
    public async Task<UserChatDTO?> GetChatDetailsAsync(Guid chatId, CancellationToken token)
    {
        if (HubConnection == null) return null;
        return await HubConnection.InvokeAsync<UserChatDTO?>("GetChatDetailsAsync", chatId, token);
    }
    public async Task RestoreChatAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("RestoreChatAsync", chatId);
    }
    public async Task<List<MessageDTO>> GetChatHistoryAsync(Guid chatId, CancellationToken token)
    {
        if (HubConnection == null) return new List<MessageDTO>();
        try
        {
            return await HubConnection.InvokeAsync<List<MessageDTO>>("GetChatHistoryAsync", chatId, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load chat messages for ChatId: {ChatId}", chatId);
            return new List<MessageDTO>();
        }
    }
    public async Task<List<InviteDTO>> GetUserInvitesAsync()
    {
        if (HubConnection == null) return new List<InviteDTO>();
        return await HubConnection.InvokeAsync<List<InviteDTO>>("GetUserInvitesAsync");
    }
    public async Task<List<ContactDTO>> GetUserContactsAsync()
    {
        if (HubConnection == null) return new List<ContactDTO>();
        return await HubConnection.InvokeAsync<List<ContactDTO>>("GetUserContactsAsync");
    }
    public async Task<ContactDTO?> GetContactByIdAsync(Guid contactId)
    {
        if (HubConnection == null) return null;
        return await HubConnection.InvokeAsync<ContactDTO?>("GetContactByIdAsync", contactId);
    }
    public async Task<List<UserDTO>> GetUsersToInviteAsync(string query)
    {
        if (HubConnection == null) return new List<UserDTO>();
        return await HubConnection.InvokeAsync<List<UserDTO>>("GetUsersToInviteAsync", query);
    }
    public async Task SendContactInviteAsync(Guid receiverId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("SendContactInviteAsync", receiverId);
    }
    public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid contactId)
    {
        if (HubConnection == null) return false;
        return await HubConnection.InvokeAsync<bool>("IsChatArchivedAsync", chatId, contactId);
    }
    public async Task HandleInviteActionAsync(Guid inviteId, bool status)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("HandleInviteActionAsync", inviteId, status);
    }
    public async Task RemoveContactAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("RemoveContactAsync", chatId);
    }
    public async Task ChangeChatNameAsync(Guid chatId, string chatName)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("ChangeChatNameAsync", chatId, chatName);
    }
    public async Task<List<ContactDTO>> GetContactListAsync()
    {
        if (HubConnection == null) return new List<ContactDTO>();
        return await HubConnection.InvokeAsync<List<ContactDTO>>("GetContactListAsync");
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
    public async Task DeleteChatAsync(Guid chatId)
    {
        if (HubConnection == null) return;
        await HubConnection.InvokeAsync("DeleteChatAsync", chatId);
    }
    public async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}
