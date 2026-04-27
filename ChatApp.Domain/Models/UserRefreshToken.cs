namespace ChatApp.Domain.Models
{
    public class UserRefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}