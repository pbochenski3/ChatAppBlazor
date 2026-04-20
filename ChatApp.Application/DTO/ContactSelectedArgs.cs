using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public record ContactSelectedArgs(
        Guid ChatId,
        bool Force = false);
        
}
