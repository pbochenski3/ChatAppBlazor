using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;


namespace ChatApp.ChatServer.Client.Services
{
    public class AppStateService
    {
        public string? Message { get; set; }

        public UserDTO? CurrentUser { get; set; } = null;
        public UserChatDTO? CurrentChat { get; set; } = null;
        public void Logout()
        {
            CurrentUser = null;
            Message = "You have been logged out.";
        }
    }
}
