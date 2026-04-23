using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Chats;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Sidebar.GetSidebarItems
{
    public record GetSidebarItemsQuery(Guid UserId) : IQuery<List<UserChatDTO>>;
    
}
