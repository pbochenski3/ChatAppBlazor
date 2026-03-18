using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatApp.Application.Mappers
{
    public static class SidebarMapper
    {
        public static SidebarDTO MapToSidebar(this ContactDTO contactDTO, int unreadCount)
        {
            return new SidebarDTO(
                 Id: contactDTO.ContactUserID,
                 DisplayName: contactDTO.Username,
                 AvatarUrl: contactDTO.AvatarUrl,
                 ItemType: EnumType.Contact,
                 IsOnline: false,
                 Counter: unreadCount
             );
        }
        public static SidebarDTO MapToSidebar(this ChatDTO chat, int unreadCount)
        {
            return new SidebarDTO(
                Id: chat.ChatID,
                DisplayName: chat.ChatName,
                AvatarUrl: chat.AvatarUrl,
                ItemType: EnumType.Chat,
                IsOnline: false,
                Counter: unreadCount
            );
        }
    }
}
