using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Feature.Files.SaveGroupAvatar
{
    public record SaveGroupAvatarCommand(IFormFile File, Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
