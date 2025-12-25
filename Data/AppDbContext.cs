using hrms_api.Enum;
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
        
        public DbSet<Permission> Permissions { get; set; }
        
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<OtpRequest> OtpRequests { get; set; }
        
        public DbSet<Attendance> Attendances { get; set; }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        public DbSet<Position> Positions { get; set; } 
        
        public DbSet<Department> Departments { get; set; }
        
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    
    modelBuilder.Entity<LeaveRequest>()
        .HasOne(l => l.ApprovedBy)
        .WithMany() // Employee can approve many leaves
        .HasForeignKey(l => l.ApprovedById)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
    
    
    // Employee → Department (many-to-one)
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Department)
        .WithMany(d => d.Employees)
        .HasForeignKey(e => e.DepartmentId)
        .OnDelete(DeleteBehavior.SetNull); // works only if DepartmentId is nullable

    // Department → Manager (one-to-one)
    modelBuilder.Entity<Department>()
        .HasOne(d => d.Manager)
        .WithMany()
        .HasForeignKey(d => d.ManagerId)
        .OnDelete(DeleteBehavior.SetNull);

    // Employee → Position (many-to-one)
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Position)
        .WithMany(p => p.Employees)
        .HasForeignKey(e => e.PositionId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<LeaveRequest>()
        .Property(l => l.LeaveStatus)
        .HasConversion<string>();
    modelBuilder.Entity<LeaveRequest>()
        .Property(l => l.LeaveType)
        .HasConversion<string>();
    modelBuilder.Entity<LeaveRequest>()
        .Property(l => l.LeaveSession)
        .HasConversion<string>();

    // Role-Permission composite key
    modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

    // Seed Roles
    var hashpassword = BCrypt.Net.BCrypt.HashPassword("superadmin123");
    modelBuilder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "SuperAdmin" },
        new Role { Id = 2, Name = "Admin" },
        new Role { Id = 3, Name = "User" }
    );

    // Seed SystemUser
    modelBuilder.Entity<SystemUser>().HasData(
        new SystemUser
        {
            Id = 1,
            Username = "superadmin",
            Password = hashpassword,
            RoleId = 1
        }
    );

    
    modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "John Doe", Email = "johndoe@example.com", Gender = Gender.Male, DOB = new DateTime(1990, 5, 12), HiredDate = new DateTime(2020, 1, 15), PhoneNumber = "1234567890", ImageUrl = "https://example.com/images/john.jpg", Address = "123 Main St" },
                new Employee { Id = 2, Name = "Jane Smith", Email = "janesmith@example.com", Gender = Gender.Female, DOB = new DateTime(1992, 8, 23), HiredDate = new DateTime(2021, 3, 1), PhoneNumber = "9876543210", ImageUrl = "https://example.com/images/jane.jpg", Address = "456 Oak St" },
                new Employee { Id = 3, Name = "Michael Johnson", Email = "michaeljohnson@example.com", Gender = Gender.Male, DOB = new DateTime(1988, 2, 10), HiredDate = new DateTime(2019, 6, 12), PhoneNumber = "5551234567", ImageUrl = "https://example.com/images/michael.jpg", Address = "789 Pine St" },
                new Employee { Id = 4, Name = "Emily Davis", Email = "emilydavis@example.com", Gender = Gender.Female, DOB = new DateTime(1995, 11, 5), HiredDate = new DateTime(2022, 2, 20), PhoneNumber = "4449876543", ImageUrl = "https://example.com/images/emily.jpg", Address = "101 Maple St" },
                new Employee { Id = 5, Name = "William Brown", Email = "williambrown@example.com", Gender = Gender.Male, DOB = new DateTime(1991, 4, 18), HiredDate = new DateTime(2020, 9, 8), PhoneNumber = "2223334444", ImageUrl = "https://example.com/images/william.jpg", Address = "202 Birch St" },
                new Employee { Id = 6, Name = "Sophia Miller", Email = "sophiamiller@example.com", Gender = Gender.Female, DOB = new DateTime(1993, 7, 29), HiredDate = new DateTime(2021, 5, 10), PhoneNumber = "3334445555", ImageUrl = "https://example.com/images/sophia.jpg", Address = "303 Cedar St" },
                new Employee { Id = 7, Name = "James Wilson", Email = "jameswilson@example.com", Gender = Gender.Male, DOB = new DateTime(1989, 12, 3), HiredDate = new DateTime(2018, 11, 15), PhoneNumber = "1112223333", ImageUrl = "https://example.com/images/james.jpg", Address = "404 Elm St" },
                new Employee { Id = 8, Name = "Olivia Martinez", Email = "oliviamartinez@example.com", Gender = Gender.Female, DOB = new DateTime(1996, 1, 14), HiredDate = new DateTime(2022, 7, 1), PhoneNumber = "6667778888", ImageUrl = "https://example.com/images/olivia.jpg", Address = "505 Walnut St" },
                new Employee { Id = 9, Name = "Daniel Anderson", Email = "danielanderson@example.com", Gender = Gender.Male, DOB = new DateTime(1994, 6, 7), HiredDate = new DateTime(2020, 4, 25), PhoneNumber = "9990001111", ImageUrl = "https://example.com/images/daniel.jpg", Address = "606 Willow St" },
                new Employee { Id = 10, Name = "Ava Thomas", Email = "avathomas@example.com", Gender = Gender.Female, DOB = new DateTime(1997, 9, 21), HiredDate = new DateTime(2023, 1, 10), PhoneNumber = "7778889999", ImageUrl = "https://example.com/images/ava.jpg", Address = "707 Cherry St" }
            );
    // Seed Permissions
    modelBuilder.Entity<Permission>().HasData(
        new Permission { PermissionId = 1, PermissionName = "View Employee" },
        new Permission { PermissionId = 2, PermissionName = "Edit Employee" },
        new Permission { PermissionId = 3, PermissionName = "Delete Employee" },
        new Permission { PermissionId = 4, PermissionName = "Generate Payroll" },
        new Permission { PermissionId = 5, PermissionName = "Approve Leave" },
        new Permission { PermissionId = 6, PermissionName = "Mark Attendance" },
        new Permission { PermissionId = 7, PermissionName = "Generate Reports" }
    );

    // Seed RolePermissions
    modelBuilder.Entity<RolePermission>().HasData(
        // SuperAdmin all permissions
        new RolePermission { RoleId = 1, PermissionId = 1 },
        new RolePermission { RoleId = 1, PermissionId = 2 },
        new RolePermission { RoleId = 1, PermissionId = 3 },
        new RolePermission { RoleId = 1, PermissionId = 4 },
        new RolePermission { RoleId = 1, PermissionId = 5 },
        new RolePermission { RoleId = 1, PermissionId = 6 },
        new RolePermission { RoleId = 1, PermissionId = 7 },

        // Admin permissions
        new RolePermission { RoleId = 2, PermissionId = 1 },
        new RolePermission { RoleId = 2, PermissionId = 2 },
        new RolePermission { RoleId = 2, PermissionId = 5 },
        new RolePermission { RoleId = 2, PermissionId = 6 },

        // Employee permissions
        new RolePermission { RoleId = 3, PermissionId = 1 },
        new RolePermission { RoleId = 3, PermissionId = 6 }
    );
}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}


