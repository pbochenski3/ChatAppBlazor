using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Domain.Enums;

namespace ChatApp.Web.Services.State
{
    public class SidebarStateService
    {
        private readonly ILogger<SidebarStateService> _logger;
        public SidebarStateService(ILogger<SidebarStateService> logger)
        {
            _logger = logger;
        }
        public bool IsPending { get; set; } = false;
        public bool IsSearchingGlobal { get; set; } = false;
        public List<UserChatDTO> SidebarItems { get; set; } = new List<UserChatDTO>();
        public List<InviteDTO> ReceivedInvites { get; set; } = new List<InviteDTO>();
        public List<UserDTO> FoundUsers { get; set; } = new List<UserDTO>();
        public SidebarView SidebarView { get; set; } = SidebarView.Contacts;

    }
}
