using ChatApp.Application.DTO;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.ChatServer.Client.Services.State;
using System.Net.Http.Json;

namespace ChatApp.ChatServer.Client.Services.Api
{
    public class AuthClient : IAuthClient
    {
        private readonly AppStateService _appStateService;
        private readonly HttpClient _httpClient;
        private readonly ChatHubService _chatHubService;
        private readonly ILogger<AuthClient> _logger;


        public AuthClient(AppStateService appStateService, HttpClient httpClient,ChatHubService chatHubService, ILogger<AuthClient> logger)
        {
            _appStateService = appStateService;
            _httpClient = httpClient;
            _chatHubService = chatHubService;
            _logger = logger;

        }
        public async Task LoginUserAsync(UserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            if (response.IsSuccessStatusCode)
            {
                var loggedInUser = await response.Content.ReadFromJsonAsync<UserDTO>();

                if (loggedInUser != null)
                {
                    _appStateService.CurrentUser = loggedInUser;
                    await _chatHubService.StartAsync();
                    await _appStateService.SetUserAsync(loggedInUser);
                    _appStateService.Message = "Logged in successfully!";
                }
                else
                {
                    throw new Exception("Logged user is null");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }
        }
        public async Task RegisterUserAsync(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
            {
                throw new ArgumentException("Username must be at least 5 characters.");
            }
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            {
                throw new ArgumentException("Password must be at least 8 characters.");
            }
            if (dto.Username.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Username is already oucppied.");
            }
            ;

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            if (response.IsSuccessStatusCode)
            {
                _appStateService.Message = "Zarejestrowano pomyślnie!";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(errorContent);
            }
        }
    }
}
