using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Message.SendChatMessage
{
    public record SendChatMessageCommand(MessageDTO Dto) : BaseCommand<bool>;
}
