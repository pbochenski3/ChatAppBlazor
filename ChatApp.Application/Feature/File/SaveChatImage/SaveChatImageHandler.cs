using ChatApp.Application.Interfaces;
using ChatApp.Domain.Enums;
using MediatR;

namespace ChatApp.Application.Feature.File.SaveChatImage
{
    public class SaveChatImageHandler : IRequestHandler<SaveChatImageCommand, string>
    {
        private readonly IFileService _fileService;
        public SaveChatImageHandler(IFileService fileService)
        {
            _fileService = fileService;
        }
        public async Task<string> Handle(SaveChatImageCommand r, CancellationToken cancellationToken)
        {
            return await _fileService.SaveImageAsync(r.File, UploadType.ChatImage, r.ChatId, r.UserId);

        }
    }
}
