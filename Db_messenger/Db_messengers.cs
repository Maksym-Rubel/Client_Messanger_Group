﻿using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;


namespace Db_messenger
{
    public class Db_messengers : DbContext
    {
        public Db_messengers()
        {
            //this.Database.EnsureDeleted();

            //this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(
                @"Server=46.201.132.38,1433;
                  Database=Group_Messenger_DB;
                  User ID=group_user;
                  Password=NewStrongPasswordHere;
                  Encrypt=False;
                  TrustServerCertificate=True;",
                sqlOptions => sqlOptions.EnableRetryOnFailure()
            );




        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Users)
                .WithMany(w => w.Chats);
            modelBuilder.Entity<Messages>()
                .HasOne(c => c.User)
                .WithMany(w => w.Messages)
                .HasForeignKey(c => c.UserId);
            modelBuilder.Entity<Messages>()
                .HasOne(c => c.Chat)
                .WithMany(w => w.Messages)
                .HasForeignKey(c => c.ChatId);

        }

        public DbSet<Messages> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }


    }
}
