using ChatApp.Application.DTO.Chats;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatDetails.GetChatDetailsHandler
{
    public class GetChatDetailsHandler : IRequestHandler<GetChatDetailsQuery, UserChatDTO>
    {
        private readonly IUserChatRepository _userChatRepo;
        public GetChatDetailsHandler(IUserChatRepository userChatRepo)
        {
            _userChatRepo = userChatRepo;
        }
        public async Task<UserChatDTO> Handle(GetChatDetailsQuery r, CancellationToken cancellationToken)
        {
            var chat = await _userChatRepo.GetUserChatAsync(r.ChatId, r.UserId, cancellationToken);

            if (chat == null) return null;

            return new UserChatDTO
            {
                Identity = new ChatIdentityDTO
                {
                    ChatID = chat.ChatID,
                    ChatName = chat.ChatName,
                    IsGroup = chat.Chat.IsGroup,
                    AvatarUrl = chat.Chat.IsGroup
                        ? chat.Chat.AvatarUrl
                        : chat.Chat.UserChats.FirstOrDefault(p => p.UserID != r.UserId)?.User?.AvatarUrl ?? "https://localhost:7255/cdn/avatars/default-avatar.png",
                    UserID = chat.UserID,
                    OtherUserId = chat.Chat.IsGroup ? null : chat.Chat.UserChats.FirstOrDefault(p => p.UserID != r.UserId)?.UserID
                },
                State = new ChatStateDTO
                {
                    IsAdmin = chat.IsAdmin,
                    IsArchive = chat.IsArchive
                }
            };
        }
    }
}
