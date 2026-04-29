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
            
            var list =  messages.Select(m => new MessageDTO
            {
                MessageID = m.MessageID,
                Content = m.Content,
                imageUrl = m.imageUrl,
                SentAt = m.SentAt,
                SenderUsername = m.Sender?.Username,
                SenderID = m.SenderID,
                ChatID = m.ChatID,
                MessageType = m.MessageType,
            }).ToList();
            if(!isGroup)
            {
            var chatId = list.First().ChatID;
            var userId = list.First().SenderID ?? Guid.CreateVersion7();
            var sender = await _userChatRepo.GetPrivateUserAliasAsync(chatId, userId);
            foreach (var message in list)
            {
                message.Alias = message.MessageType == MessageType.System ? "SYSTEM" : sender;

            }
            }
            return list;
        }
    }
}
