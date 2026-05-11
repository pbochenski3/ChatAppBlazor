using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Result;
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
        public event Action? OnStateChanged;
        public bool IsPending { get; set; } = false;
        public bool IsSearchingGlobal { get; set; } = false;
        public List<UserChatDTO> SidebarItems { get; set; } = new List<UserChatDTO>();
        public List<InviteDTO> ReceivedInvites { get; set; } = new List<InviteDTO>();
        public List<UserSearchResultDTO> FoundUsers { get; set; } = new List<UserSearchResultDTO>();
        public SidebarView SidebarView { get; set; } = SidebarView.Contacts;
        public void UpdateSidebarMessage(Guid chatId, string content)
        {
            var chat = SidebarItems.FirstOrDefault(c => c.Identity.ChatID == chatId);
            if (chat == null) return;
            if (chat.LastMessage != null)
            {
                chat.LastMessage.LastMessageContent = content;
            }
            OnStateChanged?.Invoke();
        }
        public string GetLastMessageSender(Guid ChatId)
        {
            return SidebarItems.FirstOrDefault(c => c.Identity.ChatID == ChatId)?.LastMessage?.LastMessageSender ?? string.Empty;
        }
    }
}
