namespace ChatApp.Domain.Models
{
    public class MessagePreview
    {
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public Guid? SenderId { get; set; }
    }
}
