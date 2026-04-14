using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public record InviteActionResultDto(
        Guid senderId,
        Guid receiverId,
        Guid chatId
        );
}
