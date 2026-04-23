namespace ChatApp.Application.DTO.Requests
{
    public record ChangeChatNameRequest(
        string NewName,
        string AdminName
        );

}
