using hrms_api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace hrms_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<OtpRequest> OtpRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var hashpassword = BCrypt.Net.BCrypt.HashPassword("superadmin123");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1 , Name = "SuperAdmin"},
                new Role { Id = 2 , Name = "Admin"},
                new Role { Id = 3 , Name = "User"}

                );
            modelBuilder.Entity<SystemUser>().HasData(

                new SystemUser
                {
                    Id =  1,
                    Username =  "superadmin",
                    Password = hashpassword,
                    RoleId = 1 ,
                }
                );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}


