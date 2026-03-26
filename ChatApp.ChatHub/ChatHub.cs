using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IInviteService _inviteService;
        private readonly IChatService _chatService;
        private readonly ISidebarService _sidebarService;
        private readonly ILogger<ChatHub> _logger;
        protected Guid userId => Guid.TryParse(Context.UserIdentifier, out var parseId) ? parseId : Guid.Empty;
        public ChatHub(ILogger<ChatHub> logger,
            IMessageService messageService,
            IUserService userService,
            IContactService contactService,
            IInviteService inviteService,
            IChatService chatService,
            ISidebarService sidebarService
            )
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
            _contactService = contactService;
            _inviteService = inviteService;
            _chatService = chatService;
            _sidebarService = sidebarService;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task MarkMessage(Guid chatId, Guid messageId)
        {
            await _chatService.MarkMessageAsReadAsync(userId, chatId, messageId);
        }
        public async Task MarkChatMessage(Guid chatId)
        {
            await _chatService.MarkChatMessagesAsReadAsync(userId, chatId, Context.ConnectionAborted);
        }
        public async Task<bool> CheckIfGroupExist(Guid chatId)
        {
            return await _chatService.CheckIfGroupChatExistAsync(chatId, userId);
        }
        public async Task<int> FetchUnreadCount(Guid chatId)
        {
            return await _chatService.GetUnreadCounterAsync(userId, chatId);
        }
        public async Task SendInvite(Guid receiverId)
        {
            try
            {
                await _inviteService.SendInvite(userId, receiverId);
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
        public async Task<List<UserDTO>> GetUsersToInvite(string query)
        {
            return await _userService.GetAllUsersToInvite(userId, query);
        }
        public async Task<List<ContactDTO>> GetContacts()
        {
            return await _contactService.GetUserContactsAsync(userId);
        }
        public async Task<ContactDTO> GetCurrentContacts(Guid contactId)
        {
            return await _contactService.GetContactById(contactId, userId);
        }
        public async Task SendMessage(MessageDTO dto, Guid chatId)
        {
            await _chatService.SaveLastSendedChatMessageAsync(dto.ChatID, dto.MessageID);
            await _messageService.SaveChatMessageAsync(dto);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", dto);
        }
        public async Task JoinGroupSignal(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task<HashSet<UserDTO>> GetUsersFromGroup(Guid chatId)
        {
            var userIds = await _chatService.GetListOfUsersInChatAsync(chatId);
            return await _userService.GetUsersByIdAsync(userIds);
        }
        public async Task DeleteContact(Guid chatId)
        {
            try
            {
                CancellationToken token = default;
                var contactId = await _chatService.GetReceiverUser(chatId, userId, token);
                if (contactId == Guid.Empty)
                {
                    throw new Exception();
                }
                IEnumerable<Task> tasks;
                var target = Clients.Users(contactId.ToString());
                await _contactService.DeleteContactAsync(contactId, userId, chatId);
                var UserChat = Clients.Caller.SendAsync("ChatReload", chatId);
                var TargetChat = target.SendAsync("ChatReload", chatId);
                await Task.WhenAll(UserChat, TargetChat);
                tasks = [
                   Clients.Caller.SendAsync("ReceiveStatus", "Kontakt został usunięty!"),
                   target.SendAsync("ReceiveStatus", "Ktoś usunął cie z kontaktów!"),
                   Clients.Caller.SendAsync("ContactInviteReload", true),
                   target.SendAsync("ContactInviteReload", true),

                ];
                await Task.WhenAll(tasks);
            }
            catch
            {
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred while processing the delete action.");
            }

        }
        public async Task InviteAction(Guid inviteId, bool status)
        {
            try
            {
                var senderId = await _inviteService.InviteAction(inviteId, status, userId);
                var targetUser = Clients.User(senderId.ToString());

                await Task.WhenAll(
                    Clients.Caller.SendAsync("InviteReload", true),
                    targetUser.SendAsync("InviteReload", true)
                    );
                var statusMsgCaller = status ? "Invite accepted!" : "Invite rejected.";
                var statusMsgTarget = status ? "Your invite was accepted!" : "Your invite was rejected!";

                if (status)
                {
                    await _chatService.CreatePrivateChat(userId, senderId);
                    var chatId = await _chatService.GetChatId(userId, senderId, Context.ConnectionAborted);
                    await Task.WhenAll(
                    Clients.Caller.SendAsync("SideBarReload", true),
                    targetUser.SendAsync("SideBarReload", true)
                    );
                    await Task.WhenAll(
                        Clients.Caller.SendAsync("ReceiveStatus", statusMsgCaller, senderId),
                        targetUser.SendAsync("ReceiveStatus", statusMsgTarget, userId)
                    );
                }
                else
                {
                    await Task.WhenAll(
                        Clients.Caller.SendAsync("ReceiveStatus", statusMsgCaller, senderId),
                        targetUser.SendAsync("ReceiveStatus", statusMsgTarget, userId)
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in InviteAction");
                await Clients.Caller.SendAsync("ReceiveStatus", "An error occurred.");
            }
        }
        public async Task<List<InviteDTO>> GetInvites()
        {
            return await _inviteService.GetInvites(userId);
        }
        public async Task<List<MessageDTO>> GetPrivateHistory(Guid chatId)
        {
            return await _messageService.GetPrivateHistoryAsync(userId, chatId, Context.ConnectionAborted);
        }
        public async Task<UserChatDTO> GetChat(Guid chatId)
        {
            return await _chatService.GetChatAsync(chatId, userId, Context.ConnectionAborted);
        }
        public async Task<bool> GetChatStatus(Guid ChatId, Guid ContactId)
        {
            return await _chatService.GetChatStatus(ChatId, ContactId);
        }
        public async Task<List<UserChatDTO>> GetChatList()
        {

            return await _chatService.GetChatList(userId);
        }
        public async Task<List<UserChatDTO>> GetSidebarList()
        {
            return await _sidebarService.GetSidebarItems(userId);
        }
        public async Task<List<ContactDTO>> GetContactList()
        {
            return await _contactService.GetUserContactsAsync(userId);
        }
        public async Task CreateGroupChat(Guid chatId, HashSet<Guid> usersToAdd)
        {
            await _chatService.CreateGroupChat(chatId, usersToAdd);
            IReadOnlyList<string> usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();
            await Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
            await Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);

        }
        public async Task AddUsersToGroup(Guid chatId, HashSet<Guid> usersToAdd)
        {
            var isGroupChat = await _chatService.CheckIfGroupChatExistAsync(chatId, userId);
            Guid targetChatId = chatId;

            if (!isGroupChat)
            {
                targetChatId = await _chatService.CreateGroupChat(chatId, usersToAdd);
            }
            else
            {
                await _chatService.AddUsersToGroup(chatId, usersToAdd);
            }

            var admin = await _userService.GetUserDtoAsync(userId);
            var users = await _userService.GetUsersByIdAsync(usersToAdd);

            string joinedNames = string.Join(", ", users.Select(u => u.Username));

            var systemMessage = new MessageDTO
            {
                ChatID = targetChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{admin.Username} dodał użytkowników: {joinedNames} do czatu.",
                SenderUsername = "SYSTEM",
                IsSystemMessage = true,
                SentAt = DateTime.UtcNow,
            };

            await _messageService.SaveChatMessageAsync(systemMessage);

            var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();


            // Odświeżenie sidebaru u wszystkich
            await Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);
            await Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
            await Clients.Group(targetChatId.ToString()).SendAsync("SideBarReload", true);

            // Powiadomienie o nowej wiadomości systemowej w nowej grupie
            await Clients.Group(targetChatId.ToString()).SendAsync("ReceiveMessage", systemMessage);

            // Odświeżenie stanu czatu u nowo dodanych osób (jeśli miały go otwartego jako archiwum)
            // Dzięki wcześniejszej zmianie w BlazorHub.razor, to przeładuje czat TYLKO jeśli 
            // użytkownik ma go już wybranego, więc nie "przeniesie" go siłą z innej rozmowy.
            await Clients.Users(usersToNotify).SendAsync("ChatReload", targetChatId, true);

            // Odświeżenie listy użytkowników (jeśli ktoś już ma otwarty ten czat, np. przy dodawaniu do istniejącej grupy)
            await Clients.Group(targetChatId.ToString()).SendAsync("UsersInChatReload", targetChatId);
        }
        public async Task<HashSet<Guid>> GetUsersInChat(Guid chatId)
        {
            return await _chatService.GetListOfUsersInChatAsync(chatId);
        }
        public async Task LeaveChat(Guid chatId)
        {
            var user = await _userService.GetUserDtoAsync(userId);
            var systemMessage = new MessageDTO
            {
                ChatID = chatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{user.Username} opuścił czat!",
                IsSystemMessage = true,
                SenderUsername = "SYSTEM",
                SentAt = DateTime.UtcNow,
            };
            await _messageService.SaveChatMessageAsync(systemMessage);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", systemMessage);
            await Clients.Group(chatId.ToString()).SendAsync("UsersInChatReload", chatId);
            await _chatService.ArchiveUserGroupChat(chatId, userId);
            await Clients.Caller.SendAsync("ChatReload", chatId, true);
            await Clients.Caller.SendAsync("ReceiveStatus", "Opuściłeś czat!");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());

        }
        public async Task DeleteChat(Guid chatId)
        {
            await _chatService.DeleteChatAsync(chatId, userId);
            await Clients.Caller.SendAsync("ReceiveStatus", "Czat został usunięty!");
            await Clients.Caller.SendAsync("SideBarReload", true);
            await Clients.Caller.SendAsync("ChatReload", true);
        }

    }
}
