using ChatApp.Application.DTO;
using System.Xml.Linq;

namespace ChatApp.ChatServer.Client.Services.State
{
    public class ChatSettingsService
    {
        public List<ContactDTO> ReceivedContacts { get; set; } = new List<ContactDTO>();
        public HashSet<UserDTO> UsersInChat { get; set; } = new HashSet<UserDTO>();
        public bool IsSettingsOpen { get; set;  }
        public bool IsArchive { get; set; } = false;

        public event Func<string, string, Func<Task>, Task>? OnConfirmationActionRequested;
        public event Func<string, Task>? OnMessageRequested;
        public event Func<string, Task>? OnChatNameChangeRequested;
        public event Func<Task>? OnContactLoadRequested;
        public event Func<Task>? OnContactDeleteRequested;
        public event Func<Task>? OnChatLeaveRequested;
        public event Func<Task>? OnChatDeleteRequested;
        public event Func<HashSet<Guid>,Task>? OnCreateGroupChatRequested;
        public void ToggleSettings()
        {
            IsSettingsOpen = !IsSettingsOpen;
        }
        public async Task RequestSendMessageAsync(string message)
        {
                await (OnMessageRequested.Invoke(message) ?? Task.CompletedTask);
        }
        public async Task RequestChatNameChangeAsync(string name)
        {
 
                await (OnChatNameChangeRequested.Invoke(name) ?? Task.CompletedTask);

        }
        public async Task RequestContactDeleteChangeAsync()
        {
            await (OnContactDeleteRequested?.Invoke() ?? Task.CompletedTask);
        }
        public async Task RequestCreateGroupChatAsync(HashSet<Guid> usersToAdd)
        {
            await (OnCreateGroupChatRequested?.Invoke(usersToAdd) ?? Task.CompletedTask);
        }

        public async Task RequestLoadContactsAsync()
        {
            await (OnContactLoadRequested?.Invoke() ?? Task.CompletedTask);
        }
        public async Task RequestChatLeaveAsync()
        {
            await (OnChatLeaveRequested?.Invoke() ?? Task.CompletedTask);
        }
        public async Task RequestChatDeleteAsync()
        {
            await (OnChatDeleteRequested?.Invoke() ?? Task.CompletedTask);
        }
        public async Task RequestConfirmationActionAsync(string title, string message, Func<Task> action)
        {
            await (OnConfirmationActionRequested?.Invoke(title,message,action) ?? Task.CompletedTask);
        }

    }
}
