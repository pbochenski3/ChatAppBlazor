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
        public bool IsPhotoZoomed { get; set; }
        public bool IsEmojiOpen { get; set; }
        public bool IsArchive { get; set; } = false;
        public bool IsChatLocked { get; set; } = true;
        public string CurrentUsername { get; set; } = string.Empty;
        public string CurrentAlias { get; set; } = string.Empty;
        public string CurrentImage { get; set; } = string.Empty;
        public UserDTO CurrentUserDetails { get; set; }
        private ChatSettingsView _settingsView = ChatSettingsView.Settings;

        public ChatSettingsView SettingsView
        {
            get => _settingsView;
            set
            {
                if (_settingsView != value)
                {
                    _settingsView = value;
                    OnStateChanged?.Invoke();
                }
            }
        }

        public event Action? OnStateChanged;
        public void ToggleSettings()
        {
            IsSettingsOpen = !IsSettingsOpen;
            OnStateChanged?.Invoke();
        }
        public void ToogleImageZoom(string url)
        {
            IsPhotoZoomed = !IsPhotoZoomed;
            CurrentImage = url;
            OnStateChanged?.Invoke();
        }
        public void ToggleEmojiMenu()
        {
            IsEmojiOpen = !IsEmojiOpen;
            OnStateChanged?.Invoke();
        }
        public void SetMessageList(List<MessageDTO> messages)
        {
            ReceivedMessages = messages;
            _logger.LogDebug("CLICK PHOTO");
            OnStateChanged?.Invoke();
        }

        public void AddMessage(MessageDTO dto)
        {
            if (dto != null)
            {
                ReceivedMessages.Add(dto);
                OnStateChanged?.Invoke();
            }
        }

        public async Task UpdateUserAliasInMessages(Guid userId, string newAlias)
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

            OnStateChanged?.Invoke();
        }
        public void UpdateChatMessage(Guid messageId, string content, bool isEdited)
        {
            var message = ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            message.Content = content;
            message.IsEdited = isEdited;
            OnStateChanged?.Invoke();
        }
        public void DeleteChatMessage(Guid messageId, bool isDeleted)
        {
            var message = ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            message.Content = "Wiadomość została usunięta";
            message.IsDeleted = isDeleted;
            OnStateChanged?.Invoke();

        }
    }
}
