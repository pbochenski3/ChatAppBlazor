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
                Domain.Models.Message systemMessage;
                var admin = await _userRepo.GetByIdAsync(r.UserId);
                var usersToAdd = await _userRepo.GetUsersByIdsAsync(r.UsersToAdd);
                systemMessage = existingChat.AddMembers(admin, usersToAdd);
                await _messageRepo.AddMessageAsync(systemMessage);
                r.AddEvent(new UsersAddedToGroupChatNotification(existingChat.ChatID, systemMessage, r.UsersToAdd));
                return true;
        }
    }
}
