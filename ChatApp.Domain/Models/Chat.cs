using System.Data;
using System.Security.Cryptography;

namespace ChatApp.Domain.Models
{
    public class Chat
    {
        public Guid ChatID { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ChatName { get; set; } = string.Empty;
        public ICollection<UserChat> UserChats { get; set; } = new HashSet<UserChat>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
        public bool IsDeleted { get; set; } = false;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsGroup { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Message AddMembers(User admin, List<User> usersToAdd)
        {
            var names = string.Join(", ", usersToAdd.Select(u => u.Username));
            var content = usersToAdd.Count == 1
                ? $"{admin.Username} dodał użytkownika: {names} do czatu."
                : $"{admin.Username} dodał użytkowników: {names} do czatu.";

            foreach (var user in usersToAdd)
            {
                var existingRelation = UserChats.FirstOrDefault(uc => uc.UserID == user.UserID);
                if (existingRelation != null)
                {

                    existingRelation.IsArchive = false;
                }
                else
                {
                    UserChats.Add(new UserChat { UserID = user.UserID, ChatID = this.ChatID, Alias = user.Username });
                }
            }

            return Message.CreateSystemMessage(this.ChatID, content);
        }
        public static Chat CreatePrivateChat(User user1, User user2)
        {
            var chatId = Guid.CreateVersion7();
            var chatName = $"Chat#{RandomNumberGenerator.GetInt32(0, 100000):D5}";
            var chat = new Chat
            {
                ChatID = chatId,
                CreatedAt = DateTime.UtcNow,
                IsGroup = false,
                ChatName = chatName
            };
            chat.UserChats.Add(new UserChat
            {
                UserID = user1.UserID,
                ChatID = chatId,
                Alias = user2.Username,
            });
            chat.UserChats.Add(new UserChat
            {
                UserID = user2.UserID,
                ChatID = chatId,
                Alias = user1.Username,
            });

            return chat;

        }
        public static (Chat Chat, Message SystemMessage) CreateNewGroup(Guid UserId, List<User> membersToAdd)
        {
            var chatId = Guid.CreateVersion7();
            var chatName = $"Chat#{RandomNumberGenerator.GetInt32(0, 100000):D5}";
            var admin = membersToAdd.FirstOrDefault(u => u.UserID == UserId);
            var chat = new Chat
            {
                ChatID = chatId,
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = chatName,
                AvatarUrl = "https://localhost:7255/cdn/GroupAvatars/default-group-avatar.png"
            };


            foreach (var user in membersToAdd)
            {
                chat.UserChats.Add(new UserChat
                {
                    UserID = user.UserID,
                    ChatID = chatId,
                    Alias = user.Username
                });
            }

            var names = string.Join(", ", membersToAdd.Where(m => m.UserID != UserId).Select(u => u.Username));
            var message = Message.CreateSystemMessage(chatId, $"{admin.Username} stworzył czat z: {names}.");

            return (chat, message);
        }
    }
}
