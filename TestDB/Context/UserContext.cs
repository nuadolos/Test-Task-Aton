using Microsoft.EntityFrameworkCore;
using TestDB.Entities;

namespace TestDB.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = @"server=.\sqlexpress;database=TestDB;
                    integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Предварительное создание пользователя admin
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    Guid = Guid.NewGuid().ToString(),
                    Login = "admin",
                    Password = "asd123",
                    Name = "Владимир",
                    Gender = 1,
                    Birthday = new DateTime(2003, 5, 2),
                    Admin = true,
                    CreatedBy = "database",
                    CreatedOn = DateTime.Now
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
