using ChatApp.Application.DTO;
using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.Interfaces.Common;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class GroupChatApiClient : IGroupChatApiClient
    {
        private readonly ILogger<ChatApiClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly INotificationService _notification;
        public GroupChatApiClient(ILogger<ChatApiClient> logger, HttpClient httpClient, INotificationService notification)
        {
            _logger = logger;
            _httpClient = httpClient;
            _notification = notification;
        }
        public async Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            await _httpClient.PostAsJsonAsync($"api/groupchat/from-private-chat/{chatId}", userIdsToAdd);
        }
        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            await _httpClient.PostAsJsonAsync($"api/groupchat/{chatId}/add-users", userIdsToAdd);
        }
        public async Task<bool> LeaveGroupChatAsync(Guid chatId, string username)
        {
            var response = await _httpClient.DeleteAsync($"api/groupchat/{chatId}/{username}");
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _notification.Notify("Jesteś jedynym administratorem! Nadaj komuś uprawnienia przed opusczeniem!", NotificationType.Warning);
                return false;
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to leave group chat with ChatId: {ChatId}. Server responded with: {ErrorMessage}", chatId, errorMessage);
                return false;
            }
            _notification.Notify("Pomyślnie opuszczono czat!", NotificationType.Info);
            return true;

        }
        public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
        {
            return await _httpClient.GetFromJsonAsync<HashSet<UserDTO>>($"api/groupchat/{chatId}/users");
        }
        public async Task<bool> DeleteUserFromChat(Guid chatId, Guid userId, string removedUser, string adminName)
        {
            var url = $"/api/groupchat/{chatId}/remove/{userId}?removedUserName={Uri.EscapeDataString(removedUser)}&adminName={Uri.EscapeDataString(adminName)}";
            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
