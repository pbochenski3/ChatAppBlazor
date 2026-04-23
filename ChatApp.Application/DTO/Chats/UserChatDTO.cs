namespace ChatApp.Application.DTO.Chats
{
    public class UserChatDTO
    {
        public ChatIdentityDTO Identity { get; set; } = new();
        public ChatStateDTO State { get; set; } = new();
        public LastMessageDTO LastMessage { get; set; } = new();


    }
}
