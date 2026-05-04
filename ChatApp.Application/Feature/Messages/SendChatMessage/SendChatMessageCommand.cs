using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Messages.SendChatMessage
{
    public record SendChatMessageCommand(MessageDTO Dto) : BaseCommand<bool>;
}
