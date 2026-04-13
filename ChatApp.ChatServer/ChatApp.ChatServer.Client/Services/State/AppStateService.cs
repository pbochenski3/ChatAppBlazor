using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Text.Json;


namespace ChatApp.ChatServer.Client.Services.State
{
    public class AppStateService : ITokenProvider
    {
        private readonly IJSRuntime _js;
        private readonly NavigationManager _navManager;
        private const string UserKey = "current_user_session";
        private const string ChatKey = "current_user_chat";
        public AppStateService(IJSRuntime js, NavigationManager navManager)
        {
            _js = js;
            _navManager = navManager;
        }
        public bool IsInitialized { get; private set; } = false;
        public string? Message { get; set; }
        public UserDTO? CurrentUser { get; set; } = null;
        public UserChatDTO? CurrentChat { get; set; } = null;
        public async Task LoadSessionAsync()
        {
            IsInitialized = false;
            var user = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            var chat = await _js.InvokeAsync<string?>("localStorage.getItem", ChatKey);
            if(!string.IsNullOrEmpty(user))
                {
                CurrentUser = JsonSerializer.Deserialize<UserDTO>(user);
                };
            if (!string.IsNullOrEmpty(chat))
            {
                CurrentChat = JsonSerializer.Deserialize<UserChatDTO>(chat);
            }
            ;
            IsInitialized = true;
        }
        public async Task<string?> GetToken()
        {
            if (CurrentUser != null) return CurrentUser.Token;
            var userJson = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            if(!string.IsNullOrEmpty(userJson))
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
        public async Task SetUserAsync(UserDTO user)
        {
            CurrentUser = user;
            var json = JsonSerializer.Serialize(user);
            await _js.InvokeVoidAsync("localStorage.setItem", UserKey, json);
        }
        public async Task SetChatAsync(UserChatDTO chat)
        {
            CurrentChat = chat;
            var json = JsonSerializer.Serialize(chat);
            await _js.InvokeVoidAsync("localStorage.setItem", ChatKey, json);
        }

        public async Task Logout()
        {
            CurrentUser = null;
            CurrentChat = null;
            Message = "You have been logged out.";
            await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
            await _js.InvokeVoidAsync("localStorage.removeItem", ChatKey);
            _navManager.NavigateTo("/");

        }
    }
}

