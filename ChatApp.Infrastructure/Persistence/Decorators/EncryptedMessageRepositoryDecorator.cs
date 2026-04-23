using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository.Decorators;

namespace ChatApp.Infrastructure.Persistence.Decorators
{
    public class EncryptedMessageRepositoryDecorator : IMessageRepository
    {
        private readonly IMessageRepository _innerRepository;
        private readonly IEncryptionService _encryptionService;
        public EncryptedMessageRepositoryDecorator(
            IMessageRepository innerRepository,
            IEncryptionService encryptionService)
        {
            _innerRepository = innerRepository;
            _encryptionService = encryptionService;
        }
        public async Task AddMessageAsync(Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.Content))
            {
                message.Content = _encryptionService.Encrypt(message.Content);
            }

            await _innerRepository.AddMessageAsync(message);
        }

        public async Task<List<Message>> GetMessageHistoryAsync(Guid userId, Guid chatId, DateTime? cutoffDate, CancellationToken token)
        {
            var messages = await _innerRepository.GetMessageHistoryAsync(userId, chatId, cutoffDate, token);
            foreach (var message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message.Content))
                {
                    message.Content = _encryptionService.Decrypt(message.Content);
                }
            }
            return messages;
        }

        public async Task<Dictionary<Guid, MessagePreview>> GetMessagePreviewsAsync(List<Guid> ids)
        {
            var previews = await _innerRepository.GetMessagePreviewsAsync(ids);
            foreach (var preview in previews.Values)
            {
                if (!string.IsNullOrWhiteSpace(preview.Content))
                {
                    preview.Content = _encryptionService.Decrypt(preview.Content);
                }
            }
            return previews;
        }

    }
}
