using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Requests;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.State;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class ChatApiClient : IChatApiClient
    {
        private readonly ILogger<ChatApiClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppStateService _appState;
        public ChatApiClient(ILogger<ChatApiClient> logger, HttpClient httpClient,AppStateService appState)
        {
            _logger = logger;
            _httpClient = httpClient;
            _appState = appState;
        }
        public async Task MarkMessageAsReadAsync(Guid chatId, Guid messageId)
        {
            try
            {
                await _httpClient.PatchAsync($"api/message/chat/{chatId}/read/{messageId}", null);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark messages as read for ChatId: {ChatId}", chatId);
            }
        }
        public async Task MarkAllMessagesAsReadAsync(Guid chatId, CancellationToken token = default)
        {

            try
            {
                await _httpClient.PatchAsync($"api/message/chat/{chatId}/read-all", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark messages as read for ChatId: {ChatId}", chatId);
            }
        }
        public async Task<UserChatDTO?> GetChatDetailsAsync(Guid chatId, CancellationToken token)
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<UserChatDTO>($"/api/chat/{chatId}/details", token);
                return list ?? null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Chat details retrieval for ChatId: {ChatId} was canceled.", chatId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load chat details for ChatId: {ChatId}", chatId);
                return null;
            }
        }
        public async Task<List<MessageDTO>> GetChatMessageHistoryAsync(Guid chatId, CancellationToken ct = default)
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<MessageDTO>>($"/api/message/{chatId}/history", ct);
                return list ?? new List<MessageDTO>();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Chat history retrieval for ChatId: {ChatId} was canceled.", chatId);
                return new List<MessageDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load chat messages for ChatId: {ChatId}", chatId);
                return new List<MessageDTO>();
                //zmienic zeby wyskakiwal error a nie pusty czat
            }
        }
        public async Task<bool> IsChatExistingAsync(Guid chatId)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"/api/chat/{chatId}/existing");
        }
        public async Task DeleteChatAsync(Guid chatId)
        {
            var response = await _httpClient.DeleteAsync($"/api/chat/{chatId}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
        {
            return await _httpClient.GetFromJsonAsync<HashSet<Guid>>($"api/chat/{chatId}/usersId");
        }
        public async Task ChangeUserAliasAsync(Guid chatId,Guid adminId,string newAlias, string adminName,Guid userId,string username)
        {
            var request = new ChangeAliasRequest(adminId, newAlias, adminName,userId,username);
            var response = await _httpClient.PatchAsJsonAsync($"/api/chat/{chatId}/alias", request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Alias succesfuly changed fo chat {chatId}", chatId);
            }
            else
            {
                _logger.LogError($"Alias change for chat {chatId} FAILED", chatId, response.StatusCode);
                throw new Exception();
            }

        }
        public async Task ChangeChatNameAsync(Guid chatId, string chatName, string adminName,bool isGroup)
        {
            var request = new ChangeChatNameRequest(chatName, adminName,isGroup);
            var response = await _httpClient.PatchAsJsonAsync($"/api/chat/{chatId}/name", request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Chat name changed successfully for ChatId: {ChatId}", chatId);
            }
            else
            {
                _logger.LogError("Failed to change chat name for ChatId: {ChatId}. Status Code: {StatusCode}", chatId, response.StatusCode);
                throw new Exception();
            }
        }
    }
}
