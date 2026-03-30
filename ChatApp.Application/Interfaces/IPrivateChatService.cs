using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IPrivateChatService
    {
        Task CreatePrivateChatAsync(Guid userId1, Guid userId2);
        Task<Guid?> GetPrivateChatIdAsync(Guid userId, Guid contactUserId, CancellationToken token);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
    }
}
