using ChatApp.Application.Common.Messaging;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Feature.File.SaveChatImage
{
    public record SaveChatImageCommand(IFormFile File, Guid ChatId, Guid UserId) : ICommand<string>;
}
