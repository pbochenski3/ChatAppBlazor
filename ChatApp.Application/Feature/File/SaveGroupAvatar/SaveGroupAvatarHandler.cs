using ChatApp.Application.Feature.File.SaveChatImage;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.File.SaveGroupAvatar
{
    public class SaveGroupAvatarHandler : IRequestHandler<SaveGroupAvatarCommand, bool>
    {
        private readonly IFileService _fileService;
        private readonly IChatRepository _chatRepo;
        private readonly IMediator _mediator;
        public SaveGroupAvatarHandler(IFileService fileService,IChatRepository chatRepo,IMediator mediator)
        {
            _fileService = fileService;
            _chatRepo = chatRepo;
            _mediator = mediator;
        }
        public async Task<bool> Handle(SaveGroupAvatarCommand r, CancellationToken cancellationToken)
        {
            string avatarUrl = await _fileService.SaveImageAsync(r.File, UploadType.ChatImage, r.ChatId, r.UserId);
            await _chatRepo.UpdateGroupAvatarUrl(r.ChatId, avatarUrl);
            await _mediator.Publish(new GroupAvatarUpdatedNotification(r.ChatId, avatarUrl));
            return true;

        }
    }
}
