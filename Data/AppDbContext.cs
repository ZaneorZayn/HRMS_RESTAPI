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
            
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            // Seed default permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 1, PermissionName = "View Employee" },
                new Permission { PermissionId = 2, PermissionName = "Edit Employee" },
                new Permission { PermissionId = 3, PermissionName = "Delete Employee" },
                new Permission { PermissionId = 4, PermissionName = "Generate Payroll" },
                new Permission { PermissionId = 5, PermissionName = "Approve Leave" },
                new Permission { PermissionId = 6, PermissionName = "Mark Attendance" },
                new Permission { PermissionId = 7, PermissionName = "Generate Reports" }
                
            );
            
            // Seed default role permissions (SuperAdmin gets all permissions)
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission {  RoleId = 1, PermissionId = 1 },
                new RolePermission {  RoleId = 1, PermissionId = 2 },
                new RolePermission {  RoleId = 1, PermissionId = 3 },
                new RolePermission {  RoleId = 1, PermissionId = 4 },
                new RolePermission {  RoleId = 1, PermissionId = 5 },
                new RolePermission {  RoleId = 1, PermissionId = 6 },
                new RolePermission {  RoleId = 1, PermissionId = 7 },

                // Admin permissions
                new RolePermission { RoleId = 2, PermissionId = 1 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 2, PermissionId = 5 },
                new RolePermission { RoleId = 2, PermissionId = 6 },

                // Employee permissions
                new RolePermission {  RoleId = 3, PermissionId = 1 },
                new RolePermission {  RoleId = 3, PermissionId = 6 }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}


