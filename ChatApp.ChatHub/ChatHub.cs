using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IInviteService _inviteService;
        private readonly IChatService _chatService;
        private readonly IPrivateChatService _privateChatService;
        private readonly IGroupChatService _groupChatService;
        private readonly IUserChatService _userChatService;
        private readonly IChatReadStatusService _readStatusService;
        private readonly ISidebarService _sidebarService;
        private readonly IFileService _fileService;
        private readonly ILogger<ChatHub> _logger;

        protected Guid UserId => Guid.TryParse(Context.UserIdentifier, out var parseId) ? parseId : Guid.Empty;

        public ChatHub(ILogger<ChatHub> logger,
            IMessageService messageService,
            IUserService userService,
            IContactService contactService,
            IInviteService inviteService,
            IChatService chatService,
            IPrivateChatService privateChatService,
            IGroupChatService groupChatService,
            IUserChatService userChatService,
            IChatReadStatusService readStatusService,
            ISidebarService sidebarService,
            IFileService fileService)
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
            _contactService = contactService;
            _inviteService = inviteService;
            _chatService = chatService;
            _privateChatService = privateChatService;
            _groupChatService = groupChatService;
            _userChatService = userChatService;
            _readStatusService = readStatusService;
            _sidebarService = sidebarService;
            _fileService = fileService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task JoinChatGroupSignalAsync(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
  
        public async Task<string> GetUserAvatarUrlAsync()
        {
            return await _userService.GetAvatarUrlAsync(UserId);
        }

        //public async Task<List<UserChatDTO>> GetUserChatListAsync()
        //{
        //    return await _userChatService.GetUserChatListAsync(UserId);
        //}
        public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
        {
            return await _sidebarService.GetSidebarItemsAsync(UserId);
        }
    } 
}
