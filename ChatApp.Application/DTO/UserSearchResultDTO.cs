namespace ChatApp.Application.DTO;

public record UserSearchResultDTO(
 Guid UserID,
 string Username,
 string? AvatarUrl
);

