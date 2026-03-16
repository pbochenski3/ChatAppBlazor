using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface ISidebarItem
    {
        Guid Id {  get; }
        string DisplayName { get; }
        string? AvatarUrl { get; }
        EnumType ItemType { get; }
        bool IsOnline { get; }
    }
    public enum EnumType
    {
        Contact,Chat
    }
}
