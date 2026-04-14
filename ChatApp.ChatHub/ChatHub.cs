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
        public async Task<bool> IsGroupChatExistingAsync(Guid chatId)
        {
            return await _chatService.IsGroupChatExistingAsync(chatId, UserId);
        }
  
        public async Task<ContactDTO?> GetContactByIdAsync(Guid contactId)
        {
            return await _contactService.GetContactByIdAsync(contactId, UserId);
        }
        public async Task JoinChatGroupSignalAsync(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
        {
            var userIds = await _chatService.GetUsersInChatIdAsync(chatId);
            return await _userService.GetUsersByIdsAsync(userIds);
        }
        public async Task<string> GetUserAvatarUrlAsync()
        {
            return await _userService.GetAvatarUrlAsync(UserId);
        }
       
        public async Task<List<UserChatDTO>> GetUserChatListAsync()
        {
            return await _userChatService.GetUserChatListAsync(UserId);
        }
        public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
        {
            return await _sidebarService.GetSidebarItemsAsync(UserId);
        }
    
        public async Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> usersToAdd)
        {
            await _groupChatService.CreateGroupChatAsync(chatId, usersToAdd);
            var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();
            await Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
            await Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);
        }
        public async Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> usersToAdd)
        {
            var isGroupChat = await _chatService.IsGroupChatExistingAsync(chatId, UserId);
            Guid targetChatId = chatId;

            if (!isGroupChat)
            {
                targetChatId = await _groupChatService.CreateGroupChatAsync(chatId, usersToAdd);
            }
            else
            {
                await _groupChatService.AddUsersToGroupChatAsync(chatId, usersToAdd);
            }

            var admin = await _userService.GetUserByIdAsync(UserId);
            var users = await _userService.GetUsersByIdsAsync(usersToAdd);

            string joinedNames = string.Join(", ", users.Select(u => u.Username));

            var systemMessage = new MessageDTO
            {
                ChatID = targetChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{admin?.Username} dodał użytkowników: {joinedNames} do czatu.",
                SenderUsername = "SYSTEM",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };

            await _messageService.SaveMessageAsync(systemMessage);

            var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();

            await Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);
            await Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
            await Clients.Group(targetChatId.ToString()).SendAsync("SideBarReload", true);

            await Clients.Group(targetChatId.ToString()).SendAsync("ReceiveMessage", systemMessage);

            await Clients.Users(usersToNotify).SendAsync("ChatReload", targetChatId, true);

            await Clients.Group(targetChatId.ToString()).SendAsync("UsersInChatReload", targetChatId);
        }
        public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
        {
            return await _chatService.GetUsersInChatIdAsync(chatId);
        }
        public async Task LeaveChatGroupAsync(Guid chatId)
        {
            try
            {
                await _userChatService.ArchiveUserChatAsync(chatId, UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LeaveChatGroupAsync");
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred while trying to leave the chat.");
                return;
            }
            var tasks = new List<Task>();
            var user = await _userService.GetUserByIdAsync(UserId);
            var systemMessage = new MessageDTO
            {
                ChatID = chatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{user.Username} opuścił czat!",
                MessageType = MessageType.System,
                SenderUsername = "SYSTEM",
                SentAt = DateTime.UtcNow,
            };
            await _messageService.SaveMessageAsync(systemMessage);
            tasks.AddRange(new[]
            {
                Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", systemMessage),
                Clients.Group(chatId.ToString()).SendAsync("UsersInChatReload", chatId),
                Clients.Caller.SendAsync("ChatReload", chatId, true),
                Clients.Caller.SendAsync("ReceiveStatus", "Opuściłeś czat!"),
        });
            await Task.WhenAll(tasks);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
