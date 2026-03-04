using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<ChatLog>(entity =>
            {
                entity.HasKey(cl => cl.ChatID);

                entity.HasMany(c => c.Users)
                      .WithMany(u => u.ChatLogs)
                      .UsingEntity(j => j.ToTable("ChatLogUsers"));
            });

            mb.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserID);
            });

            mb.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.MessageID);
            });


        }
    }
}
