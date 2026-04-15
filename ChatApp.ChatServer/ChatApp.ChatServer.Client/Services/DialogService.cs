namespace ChatApp.ChatServer.Client.Services
{
    public class DialogService
    {
        public string Title { get; private set; } = "";
        public string Message { get; private set; } = "";
        public string YesButton { get; private set; } = "";
        public string NoButton { get; private set; } = "";
        public bool IsVisible { get; private set; }

        private Func<Task>? _onConfirm;

        public event Action? OnStateChanged;

        public void ShowConfirm(string title, string message,string yesButton, string noButton, Func<Task> onConfirm)
        {
            Title = title;
            Message = message;
            YesButton = yesButton;
            NoButton = noButton;
            _onConfirm = onConfirm;
            IsVisible = true;
            OnStateChanged?.Invoke();
        }

        public async Task HandleClose(bool confirmed)
        {
            IsVisible = false;
            if (confirmed && _onConfirm != null)
            {
                await _onConfirm();
            }
            _onConfirm = null;
            OnStateChanged?.Invoke();
        }
    }
}
