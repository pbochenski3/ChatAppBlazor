namespace ChatApp.Application.DTO.Result;

public record UserSearchResultDTO(
 Guid UserID,
 string Username,
 string? AvatarUrl
);

