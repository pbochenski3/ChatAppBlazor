using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Feature.User.UpdateUserAvatar
{
    public record UpdateUserAvatarCommand(IFormFile File, Guid UserId) : BaseCommand<bool>;
}
