
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
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserID);

                entity.HasMany(u => u.Messages)      
                      .WithOne(m => m.Sender)       
                      .HasForeignKey(m => m.SenderID) 
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            mb.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.MessageID);
                entity.Property(m => m.Content).IsRequired().HasMaxLength(1000);
            });
        }
    }
}

