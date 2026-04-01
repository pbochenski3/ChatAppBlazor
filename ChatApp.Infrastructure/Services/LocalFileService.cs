using ChatApp.Application.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class LocalFileService : IFileService
    {
        private readonly string _storagePath;
        private const string PublicPrefix = "/cdn/avatars";
        public LocalFileService(string storagePath )
        {
            _storagePath = storagePath;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }
        public async Task<string> SaveUserAvatarAsync(Stream fileStream, string extension)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_storagePath, fileName);

          
            using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
            
            var uriBuilder = new UriBuilder("https", "localhost", 7255, $"{PublicPrefix}/{fileName}");
            return uriBuilder.ToString();
        }
    }
}
