using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat
{
    public class AddUsersToGroupHandler : IRequestHandler<AddUsersToGroupChatCommand, bool>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMediator _mediator;
        public AddUsersToGroupHandler(IChatRepository chatRepo, IUserRepository userRepo, IMessageRepository messageRepo, IMediator mediator, IUserChatRepository userChatRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _mediator = mediator;
            _userChatRepo = userChatRepo;
        }
        public async Task<bool> Handle(AddUsersToGroupChatCommand r, CancellationToken cancellationToken)
        {
            var existingChat = await _chatRepo.FetchChatById(r.ChatId);
            var admin = await _userRepo.GetByIdAsync(r.UserId);
            var usersToAdd = await _userRepo.GetUsersByIdsAsync(r.UsersToAdd);
            Domain.Models.Chat targetChat;
            Domain.Models.Message systemMessage;
            if (existingChat.IsArchivedFor(r.UserId))
            {
                return false;
            }
            if (!existingChat.IsGroup)
            {
                var result = Domain.Models.Chat.CreateNewGroup(admin, usersToAdd);
                targetChat = result.Chat;
                systemMessage = result.SystemMessage;

                await _chatRepo.AddChatAsync(targetChat);
            }
            else
            {
                targetChat = existingChat;
                systemMessage = targetChat.AddMembers(admin, usersToAdd);

            }

            await _messageRepo.AddMessageAsync(systemMessage);

            await _mediator.Publish(new UsersAddedToGroupChatNotification(targetChat.ChatID, systemMessage, r.UsersToAdd));

            return true;
        }
    }
}
