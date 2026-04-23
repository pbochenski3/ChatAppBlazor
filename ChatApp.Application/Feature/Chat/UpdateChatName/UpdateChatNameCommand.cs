using ChatApp.Application.DTO.Requests;

namespace ChatApp.Application.Feature.Chat.UpdateChatName
{
    public record UpdateChatNameCommand(Guid ChatId, Guid UserId, ChangeChatNameRequest Request) : BaseCommand<bool>;
}
