namespace ChatApp.Application.DTO.Requests
{
    public record ChangeAliasRequest(Guid adminId, string Alias, string adminName, Guid changeUserId, string username);

}
