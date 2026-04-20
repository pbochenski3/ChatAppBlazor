using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveAvatar(Stream fileStream, string extension,UploadType type);
        Task<string> SaveChatImage(Stream fileStream, string extension, Guid? chatId,Guid? userId);
        Task<string> SaveImageAsync(IFormFile file, UploadType type, Guid? chatId = null, Guid? userId = null);
    }
}
