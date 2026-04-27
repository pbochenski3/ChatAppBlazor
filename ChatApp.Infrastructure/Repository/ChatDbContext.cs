
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserChat> UserChat { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Contact>(entity =>
            {
                entity.HasKey(c => new { c.UserID, c.ContactUserID });

                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Contacts)
                    .HasForeignKey(c => c.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ContactUser)
                    .WithMany()
                    .HasForeignKey(c => c.ContactUserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.AddedAt)
                    .HasPrecision(0);

                entity.Property(c => c.DeletedAt)
                    .HasPrecision(0);
            });
            mb.Entity<Invite>(entity =>
            {
                entity.HasKey(i => i.InviteID);

                entity.Property(i => i.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(i => i.CreatedAt)
                    .HasPrecision(0);

                entity.HasOne(i => i.Sender)
                    .WithMany(u => u.SentInvites)
                    .HasForeignKey(i => i.SenderID)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(i => i.Receiver)
                    .WithMany(u => u.ReceivedInvites)
                    .HasForeignKey(i => i.ReceiverID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            mb.Entity<User>(entity =>
                {
                    entity.HasKey(u => u.UserID);

                    entity.Property(u => u.CreatedAt)
                    .HasPrecision(0);

                    entity.HasMany(u => u.Messages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.SenderID)
                    .OnDelete(DeleteBehavior.Restrict);

                });

            mb.Entity<Message>(entity =>
            {
                entity.HasQueryFilter(m => !m.IsDeleted);
                entity.HasIndex(m => new { m.ChatID, m.SentAt });
                entity.HasIndex(m => new { m.ChatID, m.MessageID, m.SenderID })
                .HasDatabaseName("Messages_UnreadCounter");

                entity.HasKey(m => m.MessageID);
                entity.Property(m => m.Content).IsRequired()
                .HasMaxLength(1000);

                entity.Property(m => m.SentAt)
                .HasPrecision(0);
                entity.Property(m => m.DeletedAt)
                .HasPrecision(0);

            });

            mb.Entity<Chat>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasKey(c => c.ChatID);
                entity.Property(c => c.ChatName)
                .IsRequired()
                .HasMaxLength(100);
                entity.Property(c => c.CreatedAt)
                .HasPrecision(0);
                entity.Property(c => c.DeletedAt)
                .HasPrecision(0);

                entity.HasMany(c => c.Messages)
                        .WithOne(m => m.Chat)
                        .HasForeignKey(m => m.ChatID)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<UserChat>(entity =>
            {
                entity.HasQueryFilter(uc => !uc.IsDeleted);
                entity.HasIndex(uc => new { uc.UserID, uc.LastMessageAt });
                entity.HasIndex(uc => uc.UserID);

                entity.HasKey(uc => new { uc.UserID, uc.ChatID });

                entity.HasOne(uc => uc.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.UserID);

                entity.HasOne(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatID);

                entity.Property(uc => uc.JoinedAt)
                .HasPrecision(0);

                entity.Property(uc => uc.ArchivedAt)
                .HasPrecision(0);
                entity.Property(uc => uc.DeletedAt)
                .HasPrecision(0);
                entity.Property(uc => uc.LastMessageAt)
                .HasPrecision(0);
                entity.Property(uc => uc.LastReadAt)
                .HasPrecision(0);
            });
            mb.Entity<UserRefreshToken>(entity =>
                {
                    entity.HasOne(rt => rt.User)             
                    .WithMany(u => u.RefreshTokens)      
             .HasForeignKey(rt => rt.UserId)     
        .OnDelete(DeleteBehavior.Cascade); 
                    entity.HasKey(x => x.Id);

                    entity.Property(x => x.Token)
                        .IsRequired()
                        .HasMaxLength(512); 

                    entity.Property(x => x.UserId)
                        .IsRequired();

                    entity.Property(x => x.ExpiryDate)
                        .IsRequired();

                    entity.Property(x => x.CreatedByIp)
                        .HasMaxLength(45);

                    entity.Property(x => x.RevokedByIp)
                        .HasMaxLength(45);

                    entity.Property(x => x.ReplacedByToken)
                        .HasMaxLength(512);

                    entity.HasIndex(x => x.Token)
                        .IsUnique();

                    entity.HasIndex(x => x.UserId);
                });
        }
    }
}

