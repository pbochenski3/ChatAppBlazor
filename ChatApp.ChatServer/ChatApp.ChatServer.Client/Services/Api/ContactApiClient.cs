using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Services.Chats;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace ChatApp.ChatServer.Client.Services.Api
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
        public async Task RemoveContactAsync(Guid chatId)
        {
    
            await _httpClient.DeleteAsync($"api/contact/delete/by-chat/{chatId}");
        }
    }
}
