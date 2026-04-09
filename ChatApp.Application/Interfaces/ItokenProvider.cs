using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface ITokenProvider
    {
        Task<string?> GetToken();
    }
}
