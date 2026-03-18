using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface ISidebarService
    {
        Task<List<SidebarDTO>> GetSidebarItems(Guid id);
    }
}
