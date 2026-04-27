using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;


namespace ChatApp.Web.Services.State
{
    public class AppStateService 
    {
        private readonly IJSRuntime _js;
        private readonly NavigationManager _navManager;
        private const string UserKey = "current_user_session";
        private const string TokenKey = "current_access_token";
        private const string ChatKey = "current_user_chat";
        public AppStateService(IJSRuntime js, NavigationManager navManager)
        {
            _js = js;
            _navManager = navManager;
        }
        public bool IsInitialized { get; private set; } = false;
        public string? Message { get; set; }
        public string? Token { get; set; }
        public UserDTO? CurrentUser { get; set; } = null;
        public UserChatDTO? CurrentChat { get; set; } = null;
        public bool IsProfileOpen { get; set; } = false;
        public event Func<Task>? OnLogoutRequested;
        public async Task LoadSessionAsync()
        {
            IsInitialized = false;

            var userJson = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            var tokenRaw = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            if (!string.IsNullOrEmpty(userJson) && !string.IsNullOrEmpty(tokenRaw))
            {
                Token = tokenRaw;

                try
                {
                    CurrentUser = JsonSerializer.Deserialize<UserDTO>(userJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch {  }
            }

            IsInitialized = true;
        }


        public async Task SetUserSessionAsync(UserDTO user, string token)
        {
            CurrentUser = user;
            Token = token;

            var json = JsonSerializer.Serialize(CurrentUser);
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, Token);
            await _js.InvokeVoidAsync("localStorage.setItem", UserKey, json);
            Message = "Logged in successfully!";

            IsInitialized = true;
        }

        public async Task Logout()
        {
            OnLogoutRequested?.Invoke();
            CurrentUser = null;
            CurrentChat = null;
            Message = "You have been logged out.";
            await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            _navManager.NavigateTo("/", forceLoad: true);

        }
        public async Task SetChatAsync(UserChatDTO chat)
        {
            CurrentChat = chat;
        }
    }
}

