using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Results
{
    public record InviteActionResultDto(
        Guid senderId,
        Guid receiverId,
        Guid chatId
        );
}
