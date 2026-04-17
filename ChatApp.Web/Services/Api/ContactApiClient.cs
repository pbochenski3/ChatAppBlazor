using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Models;
using ChatApp.Web.Services.Api.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class ContactApiClient : IContactApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContactApiClient> _logger;

        public ContactApiClient(HttpClient httpClient, ILogger<ContactApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<List<ContactDTO>> GetContactListAsync()
        {
            var contacts = await _httpClient.GetFromJsonAsync<List<ContactDTO>>("api/contact")
                           ?? new List<ContactDTO>();
            return contacts;
        }
        public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
        {
            var sidebarItems = await _httpClient.GetFromJsonAsync<List<UserChatDTO>>("api/sidebar")
                ?? new List<UserChatDTO>();
            return sidebarItems;
        }
        public async Task<List<UserDTO>> GetSearchedUsersList(string query)
        {
            var foundUsers = await _httpClient.GetFromJsonAsync<List<UserDTO>>($"api/user/to-invite?query={query}")
                ?? new List<UserDTO>();
            return foundUsers;
        }
        public async Task RemoveContactAsync(Guid chatId)
        {
    
            await _httpClient.DeleteAsync($"api/contact/delete/by-chat/{chatId}");
        }
    }
}
