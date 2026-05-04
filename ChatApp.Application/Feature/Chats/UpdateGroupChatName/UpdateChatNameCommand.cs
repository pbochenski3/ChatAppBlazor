using ChatApp.Application.DTO.Requests;

namespace ChatApp.Application.Feature.Chats.UpdateGroupChatName
{
    public record UpdateChatNameCommand(Guid ChatId, Guid UserId, ChangeChatNameRequest Request) : BaseCommand<bool>;
}
