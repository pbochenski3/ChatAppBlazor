
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ChatApp.Domain.Models;  

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
        public DbSet<UserChat> ChatUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
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
                entity.HasKey(m => m.MessageID);
                entity.Property(m => m.Content).IsRequired()
                .HasMaxLength(1000)
                .HasPrecision(0);

            });

            mb.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.ChatID);
                entity.Property(c => c.ChatName)
                .IsRequired()
                .HasMaxLength(100)
                .HasPrecision(0);

                entity.HasMany(c => c.Messages)
                        .WithOne(m => m.Chat)
                        .HasForeignKey(m => m.ChatID)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<UserChat>(entity =>
            {
                entity.HasKey(cu => new { cu.UserID, cu.ChatID });

                entity.HasOne(cu => cu.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(cu => cu.UserID);
                
                entity.HasOne(cu => cu.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(cu => cu.ChatID);
            });
        }
    }
}

