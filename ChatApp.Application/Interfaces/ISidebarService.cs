using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface ISidebarService
    {
        Task<List<UserChatDTO>> GetSidebarItemsAsync(Guid userId);
    }
}
