using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class ChatLogDTO
    {
        public int ChatID { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<MessageDTO> Messages { get; set; } = new();
        public List<UserDTO> Users { get; set; } = new();
    }
}
