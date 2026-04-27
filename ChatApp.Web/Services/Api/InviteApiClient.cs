using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;
using ChatApp.Web.Services.Api.Interfaces;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class InviteApiClient : IInviteApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InviteApiClient> _logger;
        public InviteApiClient(IHttpClientFactory factory, ILogger<InviteApiClient> logger)
        {
            _httpClient = factory.CreateClient("MessengerAPI");
            _logger = logger;
        }
        public async Task<List<InviteDTO>> GetUserInvitesAsync()
        {
            List<InviteDTO> invites;
            try
            {
                invites = await _httpClient.GetFromJsonAsync<List<InviteDTO>>("api/invite")
                           ?? new List<InviteDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user invites.");
                invites = new List<InviteDTO>();
            }
            return invites;
        }
        public async Task SendContactInviteAsync(Guid receiverId)
        {
            await _httpClient.PostAsJsonAsync($"api/invite", receiverId);
        }
        public async Task HandleInviteActionAsync(InviteActionRequest inviteRequest)
        {
            await _httpClient.PostAsJsonAsync("api/invite/action", inviteRequest);
        }

    }
}
