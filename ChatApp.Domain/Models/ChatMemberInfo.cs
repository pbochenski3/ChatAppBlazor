namespace ChatApp.Domain.Models
{
    public record ChatMemberInfo(
    Guid UserID,
    string Username,
    string? AvatarUrl,
    bool IsOnline,
    string? Alias,
    bool IsAdmin
);
}
