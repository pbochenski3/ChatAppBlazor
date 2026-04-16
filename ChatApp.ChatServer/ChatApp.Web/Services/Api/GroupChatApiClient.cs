using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace ChatApp.ChatServer.Client.Services.Api
{
    public class GroupChatApiClient : IGroupChatApiClient
    {
        private readonly ILogger<ChatApiClient> _logger;
        private readonly HttpClient _httpClient;
        public GroupChatApiClient(ILogger<ChatApiClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
           await _httpClient.PostAsJsonAsync($"api/groupchat/from-private-chat/{chatId}", userIdsToAdd);
        }
        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            await _httpClient.PostAsJsonAsync($"api/groupchat/{chatId}/add-users", userIdsToAdd);
        }
        public async Task LeaveGroupChatAsync(Guid chatId,string username)
        {
            var response = await _httpClient.DeleteAsync($"api/groupchat/{chatId}/{username}");
            if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to leave group chat with ChatId: {ChatId}. Server responded with: {ErrorMessage}", chatId, errorMessage);
            }
        }
        public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
        {
           return await _httpClient.GetFromJsonAsync<HashSet<UserDTO>>($"api/groupchat/{chatId}/users");
        }
    }
}
