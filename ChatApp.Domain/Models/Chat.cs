using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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
                    UserChats.Add(new UserChat { UserID = user.UserID, ChatID = this.ChatID, ChatName = this.ChatName });
                }
            }

            return Message.CreateSystemMessage(this.ChatID, content);
        }
        public static (Chat Chat, Message SystemMessage) CreateNewGroup(User creator, List<User> membersToAdd)
        {
            var chatId = Guid.CreateVersion7();
            var chatName = $"Chat#{RandomNumberGenerator.GetInt32(0, 100000):D5}";

            var chat = new Chat
            {
                ChatID = chatId,
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = chatName,
                AvatarUrl = "https://localhost:7255/cdn/GroupAvatars/default-group-avatar.png"
            };

          
            var allMembers = membersToAdd.Append(creator).DistinctBy(u => u.UserID).ToList();
            foreach (var user in allMembers)
            {
                chat.UserChats.Add(new UserChat
                {
                    UserID = user.UserID,
                    ChatID = chatId,
                    ChatName = chatName
                });
            }

            var names = string.Join(", ", membersToAdd.Select(u => u.Username));
            var message = Message.CreateSystemMessage(chatId, $"{creator.Username} stworzył czat z: {names}.");

            return (chat, message);
        }
        public bool IsArchivedFor(Guid userId)
        {
            var userChat = UserChats.FirstOrDefault(uc => uc.UserID == userId);
            if (userChat == null) return true;

            return userChat.IsArchive;
        }
    }
}
