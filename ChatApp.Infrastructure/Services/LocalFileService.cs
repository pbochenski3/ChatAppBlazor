using ChatApp.Application.Interfaces;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class LocalFileService : IFileService
    {

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
            var prefix = type switch
            {
                UploadType.UserAvatar => "cdn/Avatars",
                UploadType.GroupAvatar => "cdn/GroupAvatars",
            };
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_storagePath,prefix,fileName);

          
            using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
            
            var uriBuilder = new UriBuilder("https", "localhost", 7255, $"{prefix}/{fileName}");
            return uriBuilder.ToString();
        }
        public async Task<string> SaveChatImage(Stream fileStream, string extension,Guid? chatId,Guid? userId)
        {
            if (chatId == null || userId == null)
                throw new Exception("chatId or userId missing");

            var chatFolder = Path.Combine(_storagePath,"ChatImages", chatId.ToString(), userId.ToString());
            if(!Directory.Exists(chatFolder))
            {
                Directory.CreateDirectory(chatFolder);
            }
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(chatFolder, fileName);
            using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
            var uriBuilder = new UriBuilder("https", "localhost", 7255, $"cdn/ChatImages/{chatId}/{userId}/{fileName}");
            return uriBuilder.ToString();
        }
    }
}
