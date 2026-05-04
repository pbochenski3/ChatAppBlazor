using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Feature.Users.UpdateUserAvatar
{
    public record UpdateUserAvatarCommand(IFormFile File, Guid UserId) : BaseCommand<bool>;
}
