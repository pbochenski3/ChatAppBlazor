using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class Message
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; } = null!;
        public string imageUrl { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public Guid ChatID { get; set; }
        public Chat Chat { get; set; } = null!;
        public Guid SenderID { get; set; }
        public User Sender { get; set; } = null!;
        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public MessageType MessageType { get; set; }
        public virtual ICollection<MessageHistory> History { get; set; } = null!;

        public static Message CreateSystemMessage(Guid chatId, string content, Guid? senderId = null)
        {
            return new Message
            {
                MessageID = Guid.CreateVersion7(),
                ChatID = chatId,
                Content = content,
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
                SenderID = senderId ?? Guid.Empty
            };
        }
    }
}
