using ChatApp.Application.Interfaces;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class LocalFileService : IFileService
    {
        private readonly string _baseUrl = "https://localhost:7255";
        private readonly string _storagePath;
        public LocalFileService(string storagePath )
        {
            _storagePath = storagePath;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }
        public async Task<string> SaveAvatar(Stream fileStream, string extension,UploadType type)
        {
            string folderName = type == UploadType.UserAvatar ? "Avatars" : "GroupAvatars";
            string fileName = $"{Guid.NewGuid()}{extension}";
            string directoryPath = Path.Combine(_storagePath, folderName);
            string fullPath = Path.Combine(directoryPath, fileName);
            Directory.CreateDirectory(directoryPath);

          
            using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
            return $"{_baseUrl}/cdn/{folderName}/{fileName}";
        }
        public async Task<string> SaveChatImage(Stream fileStream, string extension,Guid? chatId,Guid? userId)
        {
            if (chatId == null || userId == null)
                throw new Exception("chatId or userId missing");
            string subPath = Path.Combine("ChatImages", chatId.ToString(), userId.ToString());
            string directoryPath = Path.Combine(_storagePath, subPath);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(directoryPath, fileName);
            
            Directory.CreateDirectory(directoryPath);
            using (var destinationStream = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
            return $"{_baseUrl}/cdn/ChatImages/{chatId}/{userId}/{fileName}";
        }
    }
}
