using GotIt.MSSQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL
{
    public class GotItDbContext: DbContext
    {
        public GotItDbContext(DbContextOptions<GotItDbContext> options) : base(options) { }

        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<FeedbackEntity> Feedbacks { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ItemAttributeEntity> ItemAttributes { get; set; }
        public DbSet<ItemImageEntity> ItemImages { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<ProbablyMatchEntity> ProbablyMatches { get; set; }
        public DbSet<RequestEntity> Requests { get; set; }
        public DbSet<UserChatEntity> UserChats { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProbablyMatchEntity>(e =>
            {
                e.HasOne(p => p.Item)
                 .WithMany(p => p.ProbablyMatched)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(p => p.MatchedItem)
                 .WithMany(p => p.InverseProbablyMatched)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(p => new { p.ItemId, p.MatchedItemId }).IsUnique();
            });
            modelBuilder.Entity<ItemEntity>(e =>
            {
                e.HasOne(p => p.MatchedItem)
                 .WithOne(p => p.InverseMatched)
                 .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<RequestEntity>(e =>
            {
                e.HasOne(p => p.Item)
                 .WithMany(p => p.Requests)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(p => p.Sender)
                 .WithMany(p => p.SentRequests)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(p => p.Receiver)
                 .WithMany(p => p.ReceivedRequests)
                 .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<FeedbackEntity>(e =>
            {
                e.HasOne(p => p.User)
                 .WithMany(p => p.Feedbacks)
                 .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<UserChatEntity>(e =>
            {
                e.HasIndex(p => new { p.UserId, p.ChatId }).IsUnique();
            });
            modelBuilder.Entity<CommentEntity>(e =>
            {
                e.HasOne(p => p.User)
                 .WithMany(p => p.Comments)
                 .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<UserEntity>(e=>
            {
                e.HasIndex(m => m.Email).IsUnique();
            });
            modelBuilder.Entity<NotificationEntity>(e =>
            {
                e.HasOne(p => p.Sender)
                 .WithMany(p => p.SentNotifications)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(p => p.Receiver)
                 .WithMany(p => p.ReceivedNotifications)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
