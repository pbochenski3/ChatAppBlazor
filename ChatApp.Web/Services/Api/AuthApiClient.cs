using ChatApp.Application.DTO;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.State;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ChatApp.Web.Services.Api
{
    public class AuthApiClient : IAuthClient
    {
        private readonly AppStateService _appStateService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiClient> _logger;



        public AuthApiClient(AppStateService appStateService, IHttpClientFactory factory, ILogger<AuthApiClient> logger)
        {
            _appStateService = appStateService;
            _httpClient = factory.CreateClient("MessengerAPI");
            _logger = logger;

        }
        public async Task<string?> RefreshTokenAsync()
        {
            var result = await _httpClient.PostAsync("api/auth/refresh", null);

            if (result.IsSuccessStatusCode)
            {
                var authData = await result.Content.ReadFromJsonAsync<AuthResponse>();
                if (authData != null)
                {
                    await _appStateService.SetUserSessionAsync(authData.User, authData.AccessToken);
                    return authData.AccessToken;
                }
            }

            await _appStateService.Logout();
            return null;
        }
        public async Task LoginUserAsync(UserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResult != null)
                {
                    var token = authResult.AccessToken;
                    await _appStateService.SetUserSessionAsync(authResult.User, authResult.AccessToken);
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
