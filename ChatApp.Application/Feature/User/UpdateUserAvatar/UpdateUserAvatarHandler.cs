using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.User;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.User.UpdateUserAvatar
{
    public class UpdateUserAvatarHandler : IRequestHandler<UpdateUserAvatarCommand, bool>
    {
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepo;
        public UpdateUserAvatarHandler(IFileService fileService, IUserRepository userRepo)
        {
            _fileService = fileService;
            _userRepo = userRepo;
        }
        public async Task<bool> Handle(UpdateUserAvatarCommand r, CancellationToken cancellationToken)
        {
            var avatarUrl = await _fileService.SaveImageAsync(r.File, UploadType.UserAvatar);
            if (string.IsNullOrWhiteSpace(avatarUrl) || !Uri.IsWellFormedUriString(avatarUrl, UriKind.RelativeOrAbsolute))
            {
                return false;
            }
            await _userRepo.UpdateAvatarAsync(r.UserId, avatarUrl);
            r.AddEvent(new UserAvatarUploadedNotification(r.UserId, avatarUrl));
            return true;
        }
    }
}
