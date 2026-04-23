using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTO.Requests
{
    public record InviteActionRequest(
        Guid InviteId,
        InviteStatus Response,
        Guid chatId = default

    );
}
