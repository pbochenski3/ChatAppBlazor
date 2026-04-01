using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveUserAvatarAsync(Stream fileStream, string extension);
    }
}
