using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.ExtDirect.Demo
{
    public class ChatMessage
    { 
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; } = false;
    }

    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ChatMessage> Messages { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var chatMessageEntity =
                modelBuilder
                .Entity<ChatMessage>()
                .ToTable("Messages");
            chatMessageEntity.HasKey(row => row.Id);
            chatMessageEntity.Property(row => row.Id).IsRequired().ValueGeneratedOnAdd();

            var personEntity =
                modelBuilder
                .Entity<Person>()
                .ToTable("Persons");
            personEntity.HasKey(row => row.Id);
            personEntity.Property(row => row.Id).IsRequired().ValueGeneratedOnAdd();
            personEntity.Ignore(row => row.Address);
            personEntity.Ignore(row => row.Phones);
        }
    }
}