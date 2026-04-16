using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace ChatApp.ChatServer.Client.Services.Api.Interfaces
{
    public interface IImageApiClient
    {
        Task<IBrowserFile?> GetBrowserFileAsync(InputFileChangeEventArgs e);
        Task<string> GetImageStringAsync(IBrowserFile file);
        Task<byte[]> GetFileDataAsync(IBrowserFile file);
        Task<string> UploadImageAsync(byte[] fileData, string fileName, string token, UploadType type, Guid? targetId = null);

    }
}
