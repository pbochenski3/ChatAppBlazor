using ChatApp.Application.DTO;
using ChatApp.Web.Services.Api.Interfaces;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class GroupChatApiClient : IGroupChatApiClient
    {
        private readonly ILogger<ChatApiClient> _logger;
        private readonly HttpClient _httpClient;
        public GroupChatApiClient(ILogger<ChatApiClient> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _httpClient = factory.CreateClient("MessengerAPI");

        }
        public async Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            await _httpClient.PostAsJsonAsync($"api/groupchat/from-private-chat/{chatId}", userIdsToAdd);
        }
        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            await _httpClient.PostAsJsonAsync($"api/groupchat/{chatId}/add-users", userIdsToAdd);
        }
        public async Task LeaveGroupChatAsync(Guid chatId, string username)
        {
            var response = await _httpClient.DeleteAsync($"api/groupchat/{chatId}/{username}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to leave group chat with ChatId: {ChatId}. Server responded with: {ErrorMessage}", chatId, errorMessage);
                throw new Exception(errorMessage);
            }
        }
        public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
        {
            return await _httpClient.GetFromJsonAsync<HashSet<UserDTO>>($"api/groupchat/{chatId}/users");
        }
    }
}
