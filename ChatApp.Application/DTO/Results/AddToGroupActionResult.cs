using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Results
{
    public record AddToGroupActionResult(
        Guid GroupChatId,
        MessageDTO SystemMessage
        );
}
