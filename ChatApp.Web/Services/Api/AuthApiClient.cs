using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Result;
using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.Interfaces.Common;
using ChatApp.Web.Services.State;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace ChatApp.Web.Services.Api
{
    public class AuthApiClient : IAuthClient
    {
        private readonly AppStateService _appStateService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiClient> _logger;
        private readonly INotificationService _notify;
        private readonly NavigationManager _nav;


        public AuthApiClient(AppStateService appStateService, HttpClient httpClient, ILogger<AuthApiClient> logger,INotificationService notify,NavigationManager nav)
        {
            _appStateService = appStateService;
            _httpClient = httpClient;
            _logger = logger;
            _notify = notify;
            _nav = nav;

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
                _notify.Notify("Nazwa użytkownika musi składać się z co najmniej 5 znaków.", NotificationType.Warning);
            }
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            {
                _notify.Notify("Hasło musi składać się z co najmniej 8 znaków.", NotificationType.Warning);

            }
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 20)
            {
                _notify.Notify("Nazwa użytkownika nie może przekraczać 20 znaków.", NotificationType.Warning);
            }
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length > 128)
            {
                _notify.Notify("Hasło nie może przekraczać 128 znaków.", NotificationType.Warning);

            }
            if (dto.Username.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
            {
                _notify.Notify("Podana nazwa użytkownika jest już zajęta.", NotificationType.Warning);

            }
            ;

            var response = await _httpClient.PostAsJsonAsync<UserDTO>("api/auth/register", dto);
            var responseMessage = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                _notify.Notify(responseMessage, NotificationType.Info);
                _nav.NavigateTo("/");
            }
            else
            {
            _notify.Notify(responseMessage, NotificationType.Warning);
            }
        }
    }
}
