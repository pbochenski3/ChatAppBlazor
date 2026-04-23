using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;

namespace ChatApp.Web.Services.Api.Interfaces
{
    public interface IInviteApiClient
    {
        Task<List<InviteDTO>> GetUserInvitesAsync();
        Task SendContactInviteAsync(Guid receiverId);
        Task HandleInviteActionAsync(InviteActionRequest request);
    }
}
