using ChatApp.Application.DTO;
using ChatApp.Domain.Enums;

namespace ChatApp.Web.Services.State
{
    public class ChatStateService
    {
        private readonly ILogger<ChatStateService> _logger;
        public ChatStateService(ILogger<ChatStateService> logger)
        {
            _logger = logger;
        }
        public List<ContactDTO> ReceivedContacts { get; set; } = new List<ContactDTO>();
        public HashSet<UserDTO> UsersInChat { get; set; } = new HashSet<UserDTO>();
        public List<MessageDTO> ReceivedMessages { get; private set; } = new List<MessageDTO>();

        public bool IsSettingsOpen { get; set; }
        public bool IsArchive { get; set; } = false;
        public bool IsChatLocked { get; set; } = true;
        public string CurrentUsername { get; set; } = string.Empty;
        public string CurrentAlias { get; set; } = string.Empty;
        public Guid CurrentUserDetailsId;
        public bool IsAdmin { get; set; } = false;
        public ChatSettingsView SettingsView { get; set; } = ChatSettingsView.Settings;
        public event Action? OnStateChanged;
        public void ToggleSettings()
        {
            IsSettingsOpen = !IsSettingsOpen;
            _logger.LogDebug("ToggleSettings OnStateChanged. InstanceHash={Hash}, IsSettingsOpen={IsSettingsOpen}", this.GetHashCode(), IsSettingsOpen);
            OnStateChanged?.Invoke();
        }
        public void SetMessageList(List<MessageDTO> messages)
        {
            ReceivedMessages = messages;
            _logger.LogDebug("SetMessageList OnStateChanged. InstanceHash={Hash}, MessagesCount={MessagesCount}", this.GetHashCode(), messages?.Count);
            OnStateChanged?.Invoke();
        }
        public void AddMessage(MessageDTO dto)
        {
            if (dto != null)
            {
                ReceivedMessages.Add(dto);
                _logger.LogDebug("AddMessage OnStateChanged. InstanceHash={Hash}, MessageId={MessageId}", this.GetHashCode(), dto.MessageID);
                OnStateChanged?.Invoke();
            }
        }

        public void UpdateUserAliasInMessages(Guid userId, string newAlias)
        {
            var messagesToUpdate = ReceivedMessages.Where(m => m.SenderID == userId).ToList();
            foreach (var message in messagesToUpdate)
            {
                message.Alias = newAlias;
            }

            var userToUpdate = UsersInChat.FirstOrDefault(u => u.UserID == userId);
            if (userToUpdate != null)
            {
                userToUpdate.Alias = newAlias;
            }

            _logger.LogDebug("UpdateUserAliasInMessages OnStateChanged. UserId={UserId}, NewAlias={NewAlias}", userId, newAlias);
            OnStateChanged?.Invoke();
        }

    }
}
