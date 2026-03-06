
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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Invite> Invites { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Contact>(entity =>
            {
                entity.HasKey(c => new { c.UserID, c.ContactUserID });

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Contacts)
                    .HasForeignKey(c => c.UserID)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(c => c.ContactUser)
                    .WithMany() 
                    .HasForeignKey(c => c.ContactUserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            mb.Entity<Invite>(entity =>
            {
                entity.HasKey(i => i.InviteID);

                entity.Property(i => i.Status)
                    .HasConversion<string>() 
                    .HasMaxLength(20);
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

