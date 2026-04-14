using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Requests
{
    public record ChangeChatNameRequest(
        string NewName,
        string AdminName
        );

}
