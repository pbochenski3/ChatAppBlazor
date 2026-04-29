using ChatApp.Application.DTO;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Message.GetChatMessageHistory
{
    public class GetChatHistoryHandler : IRequestHandler<GetChatMessageHistoryQuery, List<MessageDTO>>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        public GetChatHistoryHandler(IMessageRepository messageRepo, IUserChatRepository userChatRepo,IChatRepository chatRepo)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _chatRepo = chatRepo;
        }
        public async Task<List<MessageDTO>> Handle(GetChatMessageHistoryQuery r, CancellationToken cancellationToken)
        {
            bool isArchive = await _userChatRepo.IsChatArchivedAsync(r.ChatId, r.UserId);
            bool isGroup = await _chatRepo.IsChatGroupAsync(r.ChatId);
            DateTime? cutoffDate = null;

            if (isArchive)
            {
                cutoffDate = await _userChatRepo.GetLastMessageDateAsync(r.UserId, r.ChatId);
            }

            var messages = await _messageRepo.GetMessageHistoryAsync(r.UserId, r.ChatId, cutoffDate, cancellationToken);

            if (messages == null || !messages.Any())
            {
                return new List<MessageDTO>();
            }

            var aliases = await _userChatRepo.GetChatAliasesAsync(r.ChatId);
            
            return messages.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                imageUrl = m.imageUrl,
                SentAt = m.SentAt,
                SenderUsername = m.Sender?.Username,
                SenderID = m.SenderID,
                ChatID = m.ChatID,
                MessageType = m.MessageType,
                Alias = m.MessageType == MessageType.System 
                    ? "SYSTEM" 
                    : (m.SenderID.HasValue && aliases.TryGetValue(m.SenderID.Value, out var alias) ? alias : m.Sender?.Username ?? "Użytkownik")
            }).ToList();
        }
    }
}
