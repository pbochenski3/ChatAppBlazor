using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Chats
{
    public interface IPrivateChatService
    {
        Task<Guid> CreatePrivateChatAsync(Guid userId1, Guid userId2);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
    }
}
