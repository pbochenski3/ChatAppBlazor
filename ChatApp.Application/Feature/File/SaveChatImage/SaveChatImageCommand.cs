using ChatApp.Application.Common.Messaging;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.File.SaveChatImage
{
    public record SaveChatImageCommand(IFormFile File, Guid ChatId,Guid UserId) : ICommand<string>;
}
