using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;


namespace ChatApp.Web.Services.State
{
    public class AppStateService : ITokenProvider
    {
        private readonly IJSRuntime _js;
        private readonly IMediator _mediator;
        private readonly ILogger<AppStateService> _logger;
        private readonly NavigationManager _navManager;
        private const string UserKey = "current_user_session";
        private const string ActiveChatKey = "lastChatId";
        public AppStateService(IJSRuntime js,
            NavigationManager navManager,
            IMediator mediator,
            ILogger<AppStateService> logger
            )
        {
            _js = js;
            _navManager = navManager;
            _mediator = mediator;
            _logger = logger;
        }
        public bool IsInitialized { get; private set; } = false;
        public UserDTO? CurrentUser { get; set; } = null;
        public UserChatDTO? CurrentChat { get; set; } = null;
        public bool IsProfileOpen { get; set; } = false;
        public event Func<Task>? OnLogoutRequested;
        public event Action? OnStateChanged;
        public Guid SelectedChatId { get; private set; } = Guid.Empty;
        public async Task LoadSessionAsync()
        {
            IsInitialized = false;
            var user = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            if (!string.IsNullOrEmpty(user))
            {
                CurrentUser = JsonSerializer.Deserialize<UserDTO>(user);
            }
            ;
            IsInitialized = true;
        }
        public async Task<Guid> GetSelectedChatId()
        {
            var chatId = await _js.InvokeAsync<string?>("localStorage.getItem", ActiveChatKey);
            if (!string.IsNullOrEmpty(chatId))
            {
                Guid deserializedId = JsonSerializer.Deserialize<Guid>(chatId);
                SelectedChatId = deserializedId;
                return deserializedId;
            }
            return SelectedChatId;
        }
        public async Task<string?> GetToken()
        {
            if (CurrentUser != null) return CurrentUser.Token;
            var userJson = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            if (!string.IsNullOrEmpty(userJson))
            {
                try
                {
                    CurrentUser = JsonSerializer.Deserialize<UserDTO>(userJson);
                    return CurrentUser?.Token;
                }
                catch { return null; }
            }
            return null;
        }
        public void CloseProfile()
        {
            IsProfileOpen = false;
            OnStateChanged?.Invoke();
        }
        public async Task SetUserAsync(UserDTO user)
        {
            CurrentUser = user;
            var json = JsonSerializer.Serialize(user);
            await _js.InvokeVoidAsync("localStorage.setItem", UserKey, json);
        }
        public async Task SetActiveChatAsync(Guid selectedChatId)
        {
            SelectedChatId = selectedChatId;
            var json = JsonSerializer.Serialize(selectedChatId);
            await _js.InvokeVoidAsync("localStorage.setItem", ActiveChatKey, json);

        }


        public async Task Logout()
        {
            OnLogoutRequested?.Invoke();
            CurrentUser = null;
            CurrentChat = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
            await _js.InvokeVoidAsync("localStorage.removeItem", ActiveChatKey);
            _navManager.NavigateTo("/", forceLoad: true);

        }
    }
}

