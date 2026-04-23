namespace ChatApp.Application.DTO
{
    public record ContactSelectedArgs(
        Guid ChatId,
        bool Force = false);

}
