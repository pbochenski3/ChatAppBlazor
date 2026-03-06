using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
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
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger,
            IMessageService messageService,
            IUserService userService,
            IContactService contactService,
            IInviteService inviteService
            )
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
            _contactService = contactService;
            _inviteService = inviteService;
        }
        public async Task SendInvite(Guid senderId, Guid receiverId)
        {
            try
            {
                await _inviteService.SendInvite(senderId, receiverId);
                await Clients.Caller.SendAsync("ReceiveInviteStatus", "Invite sent!");
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveInviteStatus", "An error occurred while trying to send the invite.");
            }

        }
   
        public async Task<List<UserDTO>> GetUsersToInvite(Guid currentUserId, string query)
        {
            return await _userService.GetAllUsersToInvite(currentUserId, query);
        }
        public async Task<List<ContactDTO>> GetContacts(Guid id)
        {
            return await _contactService.GetUserContactsAsync(id);
        }
        public async Task SendMessage(MessageDTO dto)
        {
            dto.SentAt = DateTime.UtcNow;
            await _messageService.SendChatMessageAsync(dto);
            await Clients.All.SendAsync("ReceiveMessage", dto);
        }
        public async Task SendMessageToUsers(string receiverConnectionId, string senderUsername, string message)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderUsername, message);
        }
        public async Task RegisterUser(UserDTO dto)
        {
            try
            {
                await _userService.Register(dto);
                await Clients.Caller.SendAsync("ReceiveRegistrationStatus", "User registered successfully.");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveRegistrationStatus", $"{ex.Message}");
            }
        }
        public async Task<List<MessageDTO>> GetHistory(int count)
        { 
            return await _messageService.GetMessagesHistoryAsync(count);
        }
        public  async Task LoginUser(UserDTO dto)
        {
            try
            {
                var user = await _userService.Login(dto);
                await Clients.Caller.SendAsync("ReceiveLoginStatus", "User logged in successfully.",user);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveLoginStatus", $"{ex.Message}", null);
            }
        }
    }
}
