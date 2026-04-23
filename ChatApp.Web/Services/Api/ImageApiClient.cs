using ChatApp.Domain.Enums;
using ChatApp.Domain.Shared;
using ChatApp.Web.Services.Api.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;


namespace ChatApp.Web.Services.Api
{
    public class ImageApiClient : IImageApiClient
    {
        private readonly HttpClient _httpClient;
        private const string UploadFieldName = "file";
        public ImageApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IBrowserFile?> GetBrowserFileAsync(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".jfif" };
                var extension = Path.GetExtension(file.Name).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    file = null;
                    return file;
                }
                file = await file.RequestImageFileAsync(extension, 250, 250);
            }
            return file;
        }
        public async Task<string> GetImageStringAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return base64;
        }
        public async Task<byte[]> GetFileDataAsync(IBrowserFile file)
        {
            if (file == null)
            {
                throw new InvalidOperationException("Nie wybrano pliku do przesłania.");
            }
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
        public async Task<string> UploadImageAsync(byte[] fileData, string fileName, string token, UploadType type, Guid? targetId = null)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(fileData), UploadFieldName, fileName);

            var url = type switch
            {
                UploadType.UserAvatar => "api/user/Avatar",
                UploadType.GroupAvatar => $"api/chat/groupAvatar?chatId={targetId}",
                UploadType.ChatImage => $"api/chat/chatImage?chatId={targetId}",
            };
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            var result = await response.Content.ReadFromJsonAsync<FileResponse>();

            return result.Url ?? string.Empty;


        }
    }
}