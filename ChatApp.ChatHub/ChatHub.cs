using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
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
        private readonly IUserChatService _userChatService;
        private readonly ISidebarService _sidebarService;
        private readonly ILogger<ChatHub> _logger;
        protected Guid userId => Guid.TryParse(Context.UserIdentifier, out var parseId) ? parseId : Guid.Empty;
        public ChatHub(ILogger<ChatHub> logger,
            IMessageService messageService,
            IUserService userService,
            IContactService contactService,
            IInviteService inviteService,
            IChatService chatService,
            IUserChatService userChatService,
            ISidebarService sidebarService
            )
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
            _contactService = contactService;
            _inviteService = inviteService;
            _chatService = chatService;
            _userChatService = userChatService;
            _sidebarService = sidebarService;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task MarkMessage(Guid chatId, Guid messageId)
        {
            await _userChatService.MarkChatAsReadAsync(userId, chatId, messageId);
        }
        public async Task<List<CounterBadge>> FetchAllUnreadCount()
        {
            return await _userChatService.GetAllUnreadCounterAsync(userId);
        }

        public async Task<int> FetchUnreadCount(Guid chatId)
        {
            return await _userChatService.GetUnreadCounterAsync(userId,chatId);
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
            catch(Exception)
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
            return await _contactService.GetContactById(contactId,userId);
        }
        public async Task SendMessage(MessageDTO dto)
        {
            dto.SenderID = userId;
            await _messageService.SendChatMessageAsync(dto);
            await _userChatService.SaveLastMessageAsync(dto.ChatID, dto.MessageID);
            await Clients.All.SendAsync("ReceiveMessage", dto);
        }
        public async Task DeleteContact(Guid contactId, Guid chatId)
        {
            try
            {
                IEnumerable<Task> tasks;
                var target = Clients.Users(contactId.ToString());
                await _contactService.DeleteContactAsync(contactId, userId, chatId);
                var UserChat = Clients.Caller.SendAsync("ChatReload", contactId);
                var TargetChat = target.SendAsync("ChatReload", userId);
                await Task.WhenAll(UserChat,TargetChat);
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
                await Task.WhenAll(
                    Clients.Caller.SendAsync("ChatReload", senderId),
                    targetUser.SendAsync("ChatReload", userId)
                    );
                var statusMsgCaller = status ? "Invite accepted!" : "Invite rejected.";
                var statusMsgTarget = status ? "Your invite was accepted!" : "Your invite was rejected!";

                if (status)
                {
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
        public async Task<List<MessageDTO>> GetPrivateHistory(Guid Id,Guid chatId)
        {
            return await _messageService.GetPrivateHistoryAsync(Id,userId,chatId);
        }
        public async Task<ChatDTO> GetChat(Guid Id)
        {
            return await _chatService.GetPrivateChatById(Id,userId);
        }
        public async Task ChatRestore(Guid ContactId)
        {
            await _chatService.GetPrivateChatById(userId,ContactId);
        }
        public async Task<bool> GetChatStatus(Guid ChatId, Guid ContactId)
        {
            return await _chatService.GetChatStatus(ChatId, ContactId);
        }
        public async Task<List<ChatDTO>> GetChatList()
        {
            return await _chatService.GetChatList(userId);
        }
        public async Task<List<SidebarDTO>> GetSidebarList()
        {
            return await _sidebarService.GetSidebarItems(userId);
        }
    }
}
