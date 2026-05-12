using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Projections;
using ChatApp.Domain.Interfaces.Decorators;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Feature.Sidebar.GetSidebarItems
{
    public class GetSidebarItemsHandler : IRequestHandler<GetSidebarItemsQuery, List<UserChatDTO>>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly IEncryptionService _encryptionService;

        public GetSidebarItemsHandler(IUserChatRepository userChatRepo, IEncryptionService encryptionService)
        {
            _userChatRepo = userChatRepo;
            _encryptionService = encryptionService;
        }

        public async Task<List<UserChatDTO>> Handle(GetSidebarItemsQuery r, CancellationToken ct)
        {
            var items = await _userChatRepo.GetUserChatsQuery()
                                           .Where(uc => uc.UserID == r.UserId && !uc.IsDeleted)
                                           .OrderByDescending(uc => uc.Chat.LastMessageAt)
                                           .ProjectToSidebar(r.UserId)
                                           .ToListAsync(ct);

            foreach (var item in items)
            {
                if (item.LastMessage != null &&
                    !string.IsNullOrEmpty(item.LastMessage.LastMessageContent) &&
                    item.LastMessage.LastMessageContent != "Brak wiadomości")
                {
                    try
                    {
                        item.LastMessage.LastMessageContent = _encryptionService.Decrypt(item.LastMessage.LastMessageContent);
                    }
                    catch (Exception)
                    {
                        item.LastMessage.LastMessageContent = "[Wiadomość zaszyfrowana]";
                    }
                }
            }

            return items;
        }
    }
}
