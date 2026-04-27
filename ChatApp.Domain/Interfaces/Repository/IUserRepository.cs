using ChatApp.Domain.Models;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query);
        Task SetUserStatusAsync(Guid id, bool status);
        Task UpdateAvatarAsync(Guid userId, string avatarUrl);
        Task<List<User>> GetUsersByIdsAsync(HashSet<Guid> ids);
        Task<string> GetAvatarUrlAsync(Guid userId);
        Task AddRefreshTokenAsync(UserRefreshToken token);
        Task<UserRefreshToken?> GetRefreshTokenWithUserAsync(string token);
    }
}
