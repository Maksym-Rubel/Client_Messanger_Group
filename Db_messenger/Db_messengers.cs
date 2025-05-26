using Db_messenger.Entities;
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
            optionsBuilder.UseSqlServer(@"Data Source=37.54.60.152,1433;
                                                Initial Catalog=Group_Messenger_DB;
                                                User ID=group_user;
                                                Password=NewStrongPasswordHere;
                                                Encrypt=False;
                                                TrustServerCertificate=True;
                                                Application Intent=ReadWrite;
                                                Multi Subnet Failover=False;",

                                            sqlOptions => sqlOptions.EnableRetryOnFailure());




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
