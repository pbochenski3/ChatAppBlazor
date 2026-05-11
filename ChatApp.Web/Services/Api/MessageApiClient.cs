using ChatApp.Web.Services.Interfaces.Api;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class MessageApiClient : IMessageApiClient
    {
        private readonly ILogger<ChatApiClient> _logger;
        private readonly HttpClient _httpClient;
        public MessageApiClient(ILogger<ChatApiClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task DeleteMessageAsync(Guid messageId, Guid chatId)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/message/{chatId}/{messageId}/delete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete message {ChatId}", chatId);
            }
        }
        public async Task EditMessageAsync(Guid messageId, Guid chatId, string content)
        {
            try
            {
                await _httpClient.PatchAsJsonAsync($"api/message/{chatId}/{messageId}/edit", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit message {ChatId}", chatId);
            }
        }
    }
}
