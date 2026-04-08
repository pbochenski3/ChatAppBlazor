using ChatApp.Domain.Enums;
using ChatApp.Domain.Shared;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class ImageService
{
    private readonly HttpClient _httpClient;
    private const string UploadFieldName = "file";

    public bool IsUploading { get; set; } = false;
    public IBrowserFile? SelectedFile { get; set; }
    public string TempImageUrl { get; set; } = string.Empty;
    public bool isUploading { get; set; }

    public ImageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ProcessSelectedFileAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null) return;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".jfif" };
        var extension = Path.GetExtension(file.Name).ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            ClearSelectedFile();
            throw new Exception("Niedozwolone rozszerzenie pliku.");
        }

        SelectedFile = await file.RequestImageFileAsync(extension, 800, 800); 
        using var stream = SelectedFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());

        TempImageUrl = $"data:{SelectedFile.ContentType};base64,{base64}";
    }

    public async Task<string> UploadImageAsync(string token, UploadType type, Guid? targetId = null)
    {
        if (SelectedFile == null) throw new InvalidOperationException("Brak pliku do wysłania.");

        IsUploading = true;
        try
        {
            using var content = new MultipartFormDataContent();
            var stream = SelectedFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(SelectedFile.ContentType);

            content.Add(streamContent, UploadFieldName, SelectedFile.Name);

            var url = type switch
            {
                UploadType.UserAvatar => "api/user/updateAvatar",
                UploadType.GroupAvatar => $"api/chat/updateGroupAvatar?chatId={targetId}",
                UploadType.ChatImage => $"api/chat/saveChatImage?chatId={targetId}",
                _ => throw new ArgumentOutOfRangeException(nameof(type), null, null)
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            var result = await response.Content.ReadFromJsonAsync<FileResponse>();


            ClearSelectedFile();

            return result.Url ?? string.Empty;
        }
        finally
        {
            IsUploading = false;
        }
    }

    public void ClearSelectedFile()
    {
        SelectedFile = null;
        TempImageUrl = string.Empty;
        IsUploading = false;
    }
}