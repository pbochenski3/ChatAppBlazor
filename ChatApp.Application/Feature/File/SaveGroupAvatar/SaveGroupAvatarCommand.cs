using ChatApp.Application.Common.Messaging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.File.SaveGroupAvatar
{
    public record SaveGroupAvatarCommand(IFormFile File, Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
