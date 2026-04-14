using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Requests
{
    public record InviteActionRequest(
        Guid InviteId,
        InviteStatus Response,
        Guid chatId  = default

    );
}
