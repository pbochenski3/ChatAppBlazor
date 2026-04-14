using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;
using ChatApp.Domain.Enums;

namespace ChatApp.ChatServer.Client.Services.Api.Interfaces
{
    public interface IInviteApiClient
    {
       Task<List<InviteDTO>> GetUserInvitesAsync();
       Task SendContactInviteAsync(Guid receiverId);
       Task HandleInviteActionAsync(InviteActionRequest request);
    }
}
