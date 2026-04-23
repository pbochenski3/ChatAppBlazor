using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Chats;

namespace ChatApp.Application.Feature.Sidebar.GetSidebarItems
{
    public record GetSidebarItemsQuery(Guid UserId) : IQuery<List<UserChatDTO>>;

}
