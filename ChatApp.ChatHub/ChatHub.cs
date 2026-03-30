using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            ISidebarService sidebarService)
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
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task MarkMessageAsReadAsync(Guid chatId, Guid messageId)
        {
            await _readStatusService.MarkMessageAsReadAsync(UserId, chatId, messageId);
        }
        public async Task MarkChatMessagesAsReadAsync(Guid chatId)
        {
            await _readStatusService.MarkChatMessagesAsReadAsync(UserId, chatId, Context.ConnectionAborted);
        }
        public async Task<bool> IsGroupChatExistingAsync(Guid chatId)
        {
            return await _chatService.IsGroupChatExistingAsync(chatId, UserId);
        }
        public async Task<int> GetUnreadMessageCountAsync(Guid chatId)
        {
            return await _readStatusService.GetUnreadMessageCountAsync(UserId, chatId);
        }
        public async Task SendContactInviteAsync(Guid receiverId)
        {
            try
            {
                await _inviteService.SendInviteAsync(UserId, receiverId);
                await Clients.Caller.SendAsync("ReceiveInviteStatus", "Invite sent!");
                var targetUser = Clients.User(receiverId.ToString());
                await targetUser.SendAsync("ReceiveInviteStatus", "You have a new invite!");
                await targetUser.SendAsync("InviteReload", true);
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred while trying to send the invite.");
            }
        }
        public async Task<List<UserDTO>> GetUsersToInviteAsync(string query)
        {
            return await _userService.GetUsersToInviteAsync(UserId, query);
        }
        public async Task<List<ContactDTO>> GetUserContactsAsync()
        {
            return await _contactService.GetUserContactsAsync(UserId);
        }
        public async Task<ContactDTO?> GetContactByIdAsync(Guid contactId)
        {
            return await _contactService.GetContactByIdAsync(contactId, UserId);
        }
        public async Task SendMessageAsync(MessageDTO dto, Guid chatId)
        {
            await _readStatusService.SaveLastSentMessageIdAsync(dto.ChatID, dto.MessageID);
            await _messageService.SaveMessageAsync(dto);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", dto);
        }
        public async Task JoinChatGroupSignalAsync(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId)
        {
            var userIds = await _chatService.GetChatUsersIdsAsync(chatId);
            return await _userService.GetUsersByIdsAsync(userIds);
        }
        public async Task RemoveContactAsync(Guid chatId)
        {
            try
            {
                CancellationToken token = default;
                var contactId = await _privateChatService.GetReceiverUserIdAsync(chatId, UserId, token);
                if (contactId == Guid.Empty)
                {
                    throw new Exception("Receiver not found");
                }
                
                var target = Clients.Users(contactId.ToString());
                await _contactService.RemoveContactAsync(contactId, UserId, chatId);
                
                var userChatTask = Clients.Caller.SendAsync("ChatReload", chatId,true);
                var targetChatTask = target.SendAsync("ChatReload", chatId,true);
                await Task.WhenAll(userChatTask, targetChatTask);

                var tasks = new List<Task>
                {
                   Clients.Caller.SendAsync("ReceiveStatus", "Kontakt został usunięty!"),
                   target.SendAsync("ReceiveStatus", "Ktoś usunął cie z kontaktów!"),
                   Clients.Caller.SendAsync("ContactInviteReload", true),
                   target.SendAsync("ContactInviteReload", true)
                };
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveContactAsync");
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred while processing the delete action.");
            }
        }
        public async Task HandleInviteActionAsync(Guid inviteId, bool status)
        {
            try
            {
                var tasks = new List<Task>();
                var senderId = await _inviteService.HandleInviteActionAsync(inviteId, status, UserId);
                var targetUser = Clients.User(senderId.ToString());

                await Task.WhenAll(
                    Clients.Caller.SendAsync("InviteReload", true),
                    targetUser.SendAsync("InviteReload", true)
                );

                var statusMsgCaller = status ? "Invite accepted!" : "Invite rejected.";
                var statusMsgTarget = status ? "Your invite was accepted!" : "Your invite was rejected!";

                if (status)
                {
                    await _privateChatService.CreatePrivateChatAsync(UserId, senderId);
                    var chatId = await _privateChatService.GetPrivateChatIdAsync(UserId, senderId, Context.ConnectionAborted);
                    
                    tasks.AddRange(new[]
                    {
                        Clients.Caller.SendAsync("SideBarReload", true),
                        targetUser.SendAsync("SideBarReload", true),
                        Clients.Caller.SendAsync("ChatReload", chatId, true),
                        targetUser.SendAsync("ChatReload", chatId, true)
                    });
                }
                await Task.WhenAll(
                      Clients.Caller.SendAsync("ReceiveStatus", statusMsgCaller, senderId),
                      targetUser.SendAsync("ReceiveStatus", statusMsgTarget, UserId)
                  );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HandleInviteActionAsync");
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred.");
            }
        }
        public async Task<List<InviteDTO>> GetUserInvitesAsync()
        {
            return await _inviteService.GetUserInvitesAsync(UserId);
        }
        public async Task<List<MessageDTO>> GetChatHistoryAsync(Guid chatId)
        {
            return await _messageService.GetChatHistoryAsync(UserId, chatId, Context.ConnectionAborted);
        }
        public async Task<UserChatDTO?> GetChatDetailsAsync(Guid chatId)
        {
            return await _userChatService.GetUserChatDetailsAsync(chatId, UserId, Context.ConnectionAborted);
        }
        public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid contactId)
        {
            return await _userChatService.IsChatArchivedAsync(chatId, contactId);
        }
        public async Task<List<UserChatDTO>> GetUserChatListAsync()
        {
            return await _userChatService.GetUserChatListAsync(UserId);
        }
        public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
        {
            return await _sidebarService.GetSidebarItemsAsync(UserId);
        }
        public async Task<List<ContactDTO>> GetContactListAsync()
        {
            return await _contactService.GetUserContactsAsync(UserId);
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
                IsSystemMessage = true,
                SentAt = DateTime.UtcNow,
            };

            await _messageService.SaveMessageAsync(systemMessage);

            var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();

            // Odświeżenie sidebaru u wszystkich
            await Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);
            await Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
            await Clients.Group(targetChatId.ToString()).SendAsync("SideBarReload", true);

            // Powiadomienie o nowej wiadomości systemowej w nowej grupie
            await Clients.Group(targetChatId.ToString()).SendAsync("ReceiveMessage", systemMessage);

            // Odświeżenie stanu czatu u nowo dodanych osób
            await Clients.Users(usersToNotify).SendAsync("ChatReload", targetChatId, true);

            // Odświeżenie listy użytkowników
            await Clients.Group(targetChatId.ToString()).SendAsync("UsersInChatReload", targetChatId);
        }
        public async Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId)
        {
            return await _chatService.GetChatUsersIdsAsync(chatId);
        }
        public async Task LeaveChatGroupAsync(Guid chatId)
        {
            var user = await _userService.GetUserByIdAsync(UserId);
            var systemMessage = new MessageDTO
            {
                ChatID = chatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{user.Username} opuścił czat!",
                IsSystemMessage = true,
                SenderUsername = "SYSTEM",
                SentAt = DateTime.UtcNow,
            };
            await _messageService.SaveMessageAsync(systemMessage);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", systemMessage);
            await Clients.Group(chatId.ToString()).SendAsync("UsersInChatReload", chatId);
            await Clients.Caller.SendAsync("ChatReload", chatId, true);
            await Clients.Caller.SendAsync("ReceiveStatus", "Opuściłeś czat!");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task DeleteChatAsync(Guid chatId)
        {
            await _chatService.DeleteChatAsync(chatId, UserId);
            await Clients.Caller.SendAsync("ReceiveStatus", "Czat został usunięty!");
            await Clients.Caller.SendAsync("SideBarReload", true);
            await Clients.Caller.SendAsync("ChatReload", chatId, true);
        }
    }
}
