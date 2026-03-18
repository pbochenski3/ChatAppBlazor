using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public record SidebarDTO(
        Guid Id,
        string DisplayName,
        string? AvatarUrl,
        EnumType ItemType,
        bool IsOnline,
        int Counter
    );
    public enum EnumType { Contact, Chat }
}
