using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public record ChangeChatNameRequest(
        string NewName,
        string AdminName
        );

}
