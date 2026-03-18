using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public record CounterBadge(
        Guid Id,
        int Counter
    );

}
