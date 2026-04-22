using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.WebRequestMethods;

namespace ChatApp.Application.Services.Chats
{
    public class GroupChatService : IGroupChatService
    {
        private readonly ITransactionProvider _transactionProvider;
        private readonly ILogger<GroupChatService> _logger;
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IUserChatService _userChatService;
        private readonly IMediator _mediator;

        public GroupChatService(
            ITransactionProvider transactionProvider,
            ILogger<GroupChatService> logger,
            IChatRepository chatRepo,
            IUserChatRepository userChatRepo,
            IUserService userService,
            IMessageService messageService,
            IUserChatService userChatService,
            IMediator mediator
            )
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _userChatRepo = userChatRepo;
            _userService = userService;
            _messageService = messageService;
            _userChatService = userChatService;
            _transactionProvider = transactionProvider;
            _mediator = mediator;

        }

        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            var usersWithHistory = await _chatRepo.GetExistingUsersInChat(chatId, userIdsToAdd);
            if (usersWithHistory != null && usersWithHistory.Any())
            {
                // restore (unarchive) existing users' entries
                await _userChatRepo.SetChatAccessibilityAsync(chatId, true, usersWithHistory);
                var usersWithoutHistory = userIdsToAdd.Where(id => !usersWithHistory.Contains(id)).ToHashSet();
                if (usersWithoutHistory.Any())
                {
                    await _chatRepo.AddUserGroupToDb(chatId, usersWithoutHistory);
                }
            }
            else
            {
                await _chatRepo.AddUserGroupToDb(chatId, userIdsToAdd);
            }
        }

        //public async Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd)
        //{
        //    var usersInGroup = await _userChatRepo.GetUsersInChatIdAsync(existingChatId);
        //    usersInGroup.UnionWith(userIdsToAdd);
        //    if (usersInGroup.Count < 3)
        //    {
        //        throw new InvalidOperationException("Chat group need more that 2 people!");
        //    }

        //    int number = RandomNumberGenerator.GetInt32(0, 100000);
        //    var newChat = new Chat
        //    {
        //        ChatID = Guid.CreateVersion7(),
        //        CreatedAt = DateTime.UtcNow,
        //        IsGroup = true,
        //        ChatName = $"Chat#{number:D5}",
        //        UserChats = new List<UserChat>(),
        //        AvatarUrl = "https://localhost:7255/cdn/GroupAvatars/default-group-avatar.png"
        //    };

        //    foreach (var userId in usersInGroup)
        //    {
        //        newChat.UserChats.Add(new UserChat
        //        {
        //            UserID = userId,
        //            ChatID = newChat.ChatID,
        //            ChatName = newChat.ChatName,
        //            IsArchive = false,
        //        });
        //    }

        //    await _chatRepo.AddChatAsync(newChat);
        //    await _mediator.Publish(new GroupChatCreatedNotification(newChat.ChatID));
        //    return newChat.ChatID;
            
        //}
        //public async Task ProcessAddToGroupChatAsync(Guid chatId, HashSet<Guid> usersToAdd, Guid userId)
        //{
        //     await _transactionProvider.ExecuteInTransactionAsync(async () =>
        //    {
        //        var IsArchive = await _chatRepo.CheckIfChatIsArchive(chatId,userId);
        //        if(IsArchive == true)
        //        {
        //            throw new Exception("Nie można usunąć czatu!");
        //        }
        //        MessageDTO systemMessage = new MessageDTO();
        //        var isGroupChat = await _chatRepo.CheckIfGroupExist(chatId, userId);
        //        Guid targetChatId = chatId;
        //        var admin = await _userService.GetUserByIdAsync(userId);
        //        HashSet<UserDTO> addedUsers = new HashSet<UserDTO>();
        //        string joinedNames = string.Empty;


        //        if (!isGroupChat)
        //        {
        //            targetChatId = await CreateGroupChatAsync(chatId, usersToAdd);
        //            var allUsersInChat = await ProccesGetChatUsersAsync(targetChatId);
        //            usersToAdd.UnionWith(allUsersInChat.Select(u => u.UserID));
        //            addedUsers = await _userService.GetUsersByIdsAsync(usersToAdd);
        //            joinedNames = string.Join(", ", addedUsers.Where(u => u.UserID != userId).Select(u => u.Username));
        //            systemMessage.Content = $"{admin?.Username} stworzył czat z: {joinedNames}.";
        //        }
        //        else
        //        {
        //            await AddUsersToGroupChatAsync(chatId, usersToAdd);
        //            addedUsers = await _userService.GetUsersByIdsAsync(usersToAdd);
        //            joinedNames = string.Join(", ", addedUsers.Where(u => u.UserID != userId).Select(u => u.Username));
        //            if (addedUsers.Count == 1)
        //            {
        //                systemMessage.Content = $"{admin?.Username} dodał użytkownika: {joinedNames} do czatu.";
        //            }
        //            else
        //                systemMessage.Content = $"{admin?.Username} dodał użytkowników: {joinedNames} do czatu.";
        //        }
        //        systemMessage.ChatID = targetChatId;
        //        systemMessage.MessageID = Guid.CreateVersion7();
        //        systemMessage.SenderUsername = "SYSTEM";
        //        systemMessage.MessageType = MessageType.System;
        //        systemMessage.SentAt = DateTime.UtcNow;

        //        await _messageService.SaveMessageAsync(systemMessage);
        //        await _mediator.Publish(new UsersAddedToGroupChatNotification(targetChatId, systemMessage, usersToAdd));

        //    });
        //}
        public async Task ProcessLeaveGroupChatAsync(Guid chatId, Guid userId, string username)
        {
             await _transactionProvider.ExecuteInTransactionAsync(async () =>
            {
                var isArchive = await _chatRepo.CheckIfChatIsArchive(chatId,userId);
                if(isArchive == true)
                {
                    throw new Exception("Chat który chcesz opuścić nie istnieje!");
                }
                await _userChatService.ArchiveUserChatAsync(chatId, userId);

                var systemMessage = new MessageDTO
                {
                    ChatID = chatId,
                    MessageID = Guid.CreateVersion7(),
                    Content = $"{username} opuścił czat.",
                    SenderUsername = "SYSTEM",
                    MessageType = MessageType.System,
                    SentAt = DateTime.UtcNow,
                };

                await _messageService.SaveMessageAsync(systemMessage);
                await _mediator.Publish(new UserLeavedGroupNotification(chatId, systemMessage, userId));
            });
        }
        //public async Task<HashSet<UserDTO>> ProccesGetChatUsersAsync(Guid chatId)
        //{
        //    var userIds = await _userChatRepo.GetUsersInChatIdAsync(chatId);
        //    return await _userService.GetUsersByIdsAsync(userIds);
        //}
    }
}
