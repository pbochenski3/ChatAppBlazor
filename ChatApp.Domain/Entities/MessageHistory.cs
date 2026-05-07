namespace ChatApp.Domain.Entities
{
    public class MessageHistory
    {
        public Guid MessageId { get; set; }
        public int Version { get; set; }
        public DateTime EditedAt { get; set; }
        public string OldContent { get; set; } = null!;

    }
}
