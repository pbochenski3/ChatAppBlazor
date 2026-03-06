using ChatApp.Application.DTO;
using Microsoft.AspNetCore.Components;


namespace ChatApp.ChatServer.Client.Services
{
    public class AppStateService
    {
        public string? Message { get; set; }

        public UserDTO? CurrentUser { get; set; }
        public void Logout()
        {
            CurrentUser = null;
            Message = "You have been logged out.";
        }
    }
}
