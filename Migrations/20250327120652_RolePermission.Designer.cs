﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using hrms_api.Data;

#nullable disable

namespace hrms_api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250327120652_RolePermission")]
    partial class RolePermission
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("hrms_api.Model.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("HiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SystemUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SystemUserId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("hrms_api.Model.OtpRequest", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("otp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("OtpRequests");
                });

            modelBuilder.Entity("hrms_api.Model.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PermissionId"));

                    b.Property<string>("PermissionName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionId = 1,
                            PermissionName = "View Employee"
                        },
                        new
                        {
                            PermissionId = 2,
                            PermissionName = "Edit Employee"
                        },
                        new
                        {
                            PermissionId = 3,
                            PermissionName = "Delete Employee"
                        },
                        new
                        {
                            PermissionId = 4,
                            PermissionName = "Create Payroll"
                        },
                        new
                        {
                            PermissionId = 5,
                            PermissionName = "Approve Leave"
                        },
                        new
                        {
                            PermissionId = 6,
                            PermissionName = "Mark Attendance"
                        },
                        new
                        {
                            PermissionId = 7,
                            PermissionName = "Generate Reports"
                        });
                });

            modelBuilder.Entity("hrms_api.Model.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 3,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("hrms_api.Model.RolePermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int?>("RolePermissionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RolePermissionId");

                    b.ToTable("RolePermissions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PermissionId = 1,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            PermissionId = 2,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 3,
                            PermissionId = 3,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 4,
                            PermissionId = 4,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 5,
                            PermissionId = 5,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 6,
                            PermissionId = 6,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 7,
                            PermissionId = 7,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 8,
                            PermissionId = 1,
                            RoleId = 2
                        },
                        new
                        {
                            Id = 9,
                            PermissionId = 2,
                            RoleId = 2
                        },
                        new
                        {
                            Id = 10,
                            PermissionId = 5,
                            RoleId = 2
                        },
                        new
                        {
                            Id = 11,
                            PermissionId = 6,
                            RoleId = 2
                        },
                        new
                        {
                            Id = 12,
                            PermissionId = 1,
                            RoleId = 3
                        },
                        new
                        {
                            Id = 13,
                            PermissionId = 6,
                            RoleId = 3
                        });
                });

            modelBuilder.Entity("hrms_api.Model.SystemUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("SystemUsers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Password = "$2a$10$UlKLQXy.0Ggx9AjzGu/A9O.tU71ux4GXt9KPOsbLyinZGeibWAStu",
                            RoleId = 1,
                            Username = "superadmin"
                        });
                });

            modelBuilder.Entity("hrms_api.Model.Employee", b =>
                {
                    b.HasOne("hrms_api.Model.SystemUser", "SystemUser")
                        .WithMany()
                        .HasForeignKey("SystemUserId");

                    b.Navigation("SystemUser");
                });

            modelBuilder.Entity("hrms_api.Model.RolePermission", b =>
                {
                    b.HasOne("hrms_api.Model.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hrms_api.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hrms_api.Model.RolePermission", null)
                        .WithMany("RolePermissions")
                        .HasForeignKey("RolePermissionId");

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("hrms_api.Model.SystemUser", b =>
                {
                    b.HasOne("hrms_api.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("hrms_api.Model.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("hrms_api.Model.RolePermission", b =>
                {
                    b.Navigation("RolePermissions");
                });
#pragma warning restore 612, 618
        }
    }
}
