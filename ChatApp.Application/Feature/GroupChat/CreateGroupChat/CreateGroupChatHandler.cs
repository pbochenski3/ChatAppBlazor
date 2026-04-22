using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.CreateGroupChat
{
    public class CreateGroupChatHandler : IRequestHandler<CreateGroupChatCommand, bool>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMediator _mediator;
        public CreateGroupChatHandler(IUserChatRepository userChatRepo, IChatRepository chatRepo, IMediator mediator)
        {
            _userChatRepo = userChatRepo;
            _chatRepo = chatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(CreateGroupChatCommand r, CancellationToken cancellationToken)
        {
            var usersInGroup = await _userChatRepo.GetUsersInChatIdAsync(r.ChatId);
            usersInGroup.UnionWith(r.UsersToAdd);
            if (usersInGroup.Count < 3)
            {
                throw new InvalidOperationException("Chat group need more that 2 people!");
            }

            int number = RandomNumberGenerator.GetInt32(0, 100000);
            var newChat = new Domain.Models.Chat
            {
                ChatID = Guid.CreateVersion7(),
                CreatedAt = DateTime.UtcNow,
                IsGroup = true,
                ChatName = $"Chat#{number:D5}",
                UserChats = new List<UserChat>(),
                AvatarUrl = "https://localhost:7255/cdn/GroupAvatars/default-group-avatar.png"
            };

            foreach (var userId in usersInGroup)
            {
                newChat.UserChats.Add(new UserChat
                {
                    UserID = userId,
                    ChatID = newChat.ChatID,
                    ChatName = newChat.ChatName,
                    IsArchive = false,
                });
            }

            await _chatRepo.AddChatAsync(newChat);
            await _mediator.Publish(new GroupChatCreatedNotification(newChat.ChatID, usersInGroup));
            return true;

        }
    }
}
