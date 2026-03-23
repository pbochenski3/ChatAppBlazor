using ChatApp.Application.DTO;
using Microsoft.AspNetCore.Components;


namespace ChatApp.ChatServer.Client.Services
{
    public class AppStateService
    {
        public string? Message { get; set; }

        public UserDTO? CurrentUser { get; set; } = null;
        public HashSet<Guid> CurrentUsers { get; set; }
        public UserChatDTO? CurrentChat { get; set; } = null;
        public void Logout()
        {
            CurrentUser = null;
            Message = "You have been logged out.";
        }
    }
}
