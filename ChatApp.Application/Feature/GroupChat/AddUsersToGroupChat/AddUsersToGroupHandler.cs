//using ChatApp.Application.DTO;
//using ChatApp.Application.Interfaces;
//using ChatApp.Application.Interfaces.Repository;
//using ChatApp.Application.Notifications.GroupChat;
//using ChatApp.Domain.Enums;
//using ChatApp.Domain.Models;
//using ChatApp.Domain.Repository;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Security.Cryptography;
//using System.Text;

//namespace ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat
//{
//    public class AddUsersToGroupHandler : IRequestHandler<AddUsersToGroupChatCommand, bool>
//    {
//        private readonly IChatRepository _chatRepo;
//        private readonly IUserRepository _userRepo;
//        private readonly IUserChatRepository _userChatRepo;
//        private readonly IMessageRepository _messageRepo;
//        private readonly IMediator _mediator;
//        public AddUsersToGroupHandler(IChatRepository chatRepo,IUserRepository userRepo, IMessageRepository messageRepo, IMediator mediator,IUserChatRepository userChatRepo)
//        {
//            _chatRepo = chatRepo;
//            _userRepo = userRepo;
//            _messageRepo = messageRepo;
//            _mediator = mediator;
//            _userChatRepo = userChatRepo;
//        }
//        public async Task<bool> Handle(AddUsersToGroupChatCommand r, CancellationToken cancellationToken)
//        {
//                var IsArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
//                if (IsArchive == true)
//                {
//                return false;
//                }
//                MessageDTO systemMessage = new MessageDTO();
//                var isGroupChat = await _chatRepo.CheckIfGroupExist(r.ChatId, r.UserId);
//                Guid targetChatId = r.ChatId;
//                var user = await _userRepo.GetByIdAsync(r.UserId);
//                var admin = user?.Username ?? "Nieznany";
//                HashSet<UserDTO> addedUsers = new HashSet<UserDTO>();
//                string joinedNames = string.Empty;


//                if (!isGroupChat)
//                {
//                //creategroupchat
//                var usersInGroup = await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
//                usersInGroup.UnionWith(r.UsersToAdd);
//                if (usersInGroup.Count < 3)
//                {
//                    throw new InvalidOperationException("Chat group need more that 2 people!");
//                }

//                int number = RandomNumberGenerator.GetInt32(0, 100000);
//                var newChat = new Domain.Models.Chat
//                {
//                    ChatID = Guid.CreateVersion7(),
//                    CreatedAt = DateTime.UtcNow,
//                    IsGroup = true,
//                    ChatName = $"Chat#{number:D5}",
//                    UserChats = new List<UserChat>(),
//                    AvatarUrl = "https://localhost:7255/cdn/GroupAvatars/default-group-avatar.png"
//                };

//                foreach (var userId in usersInGroup)
//                {
//                    newChat.UserChats.Add(new UserChat
//                    {
//                        UserID = userId,
//                        ChatID = newChat.ChatID,
//                        ChatName = newChat.ChatName,
//                        IsArchive = false,
//                    });
//                }

//                await _chatRepo.AddChatAsync(newChat);
//                //
//                    var allUsersInChat = await _userChatRepo.GetUsersInChatIdAsync(targetChatId);
//                    r.UsersToAdd.UnionWith(allUsersInChat);
//                //GetUsersByIdAsync
//                var id = await _userRepo.GetByIdAsync(r.UserId);

//                var UserDto = new UserDTO
//                {
//                    UserID = user.UserID,
//                    Username = user.Username,
//                    AvatarUrl = user.AvatarUrl,
//                    IsOnline = user.IsOnline
//                };
//                //
//                    joinedNames = string.Join(", ", addedUsers.Where(u => u.UserID != r.UserId).Select(u => u.Username));
//                    systemMessage.Content = $"{admin} stworzył czat z: {joinedNames}.";
//                }
//                else
//                {
//                //AddUsersToGroupChat
//                HashSet<Guid> Users = new HashSet<Guid>();
//                var usersWithHistory = await _chatRepo.GetExistingUsersInChat(r.ChatId, r.UsersToAdd);
//                if (usersWithHistory != null && usersWithHistory.Any())
//                {
//                    await _userChatRepo.SetChatAccessibilityAsync(r.ChatId, true, usersWithHistory);
//                    var usersWithoutHistory = r.UsersToAdd.Where(id => !usersWithHistory.Contains(id)).ToHashSet();
//                    if (usersWithoutHistory.Any())
//                    {
//                        await _chatRepo.AddUserGroupToDb(r.ChatId, usersWithoutHistory);
//                        Users = usersWithoutHistory;    
//                    }
//                    else
//                    {
//                        await _chatRepo.AddUserGroupToDb(r.ChatId, r.UsersToAdd);
//                    }
//                }
//                //

//                    joinedNames = string.Join(", ", addedUsers.Where(u => u.UserID != userId).Select(u => u.Username));
//                    if (addedUsers.Count == 1)
//                    {
//                        systemMessage.Content = $"{admin} dodał użytkownika: {joinedNames} do czatu.";
//                    }
//                    else
//                        systemMessage.Content = $"{admin} dodał użytkowników: {joinedNames} do czatu.";
//                }
//                systemMessage.ChatID = targetChatId;
//                systemMessage.MessageID = Guid.CreateVersion7();
//                systemMessage.SenderUsername = "SYSTEM";
//                systemMessage.MessageType = MessageType.System;
//                systemMessage.SentAt = DateTime.UtcNow;
//            var message = new Domain.Models.Message
//            {
//                Content = systemMessage.Content,
//                imageUrl = systemMessage.imageUrl,
//                SenderID = systemMessage.SenderID,
//                ChatID = systemMessage.ChatID,
//                MessageID = systemMessage.MessageID,
//                SentAt = systemMessage.SentAt,
//                MessageType = systemMessage.MessageType,

//            };

//            await _messageRepo.AddMessageAsync(message);
//                await _mediator.Publish(new UsersAddedToGroupChatNotification(targetChatId, systemMessage, r.UsersToAdd));

//            }
//    }
//}
