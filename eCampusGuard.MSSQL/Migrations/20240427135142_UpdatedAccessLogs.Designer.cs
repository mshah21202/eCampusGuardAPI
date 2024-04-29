﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eCampusGuard.MSSQL;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    [DbContext(typeof(SQLDataContext))]
    [Migration("20240427135142_UpdatedAccessLogs")]
    partial class UpdatedAccessLogs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.16")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AccessLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserPermitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserPermitId");

                    b.ToTable("AccessLogs");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomeScreenWidgets")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppUserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Gate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Occupied")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Areas");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Permit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int>("Days")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Occupied")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Permits", t =>
                        {
                            t.HasTrigger("LC_TRIGGER_AFTER_UPDATE_PERMIT");
                        });

                    b.HasAnnotation("LC_TRIGGER_AFTER_UPDATE_PERMIT", "CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @NewId) WHERE dbo.Areas.Id = @NewAreaId\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @OldId) WHERE dbo.Areas.Id = @OldAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.PermitApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AttendingDays")
                        .HasColumnType("int");

                    b.Property<string>("LicenseImgPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PermitId")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SiblingsCount")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermitId");

                    b.HasIndex("UserId");

                    b.HasIndex("VehicleId");

                    b.ToTable("PermitApplications");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.UpdateRequest", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("NewPermitId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UpdatedVehicleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NewPermitId");

                    b.HasIndex("UpdatedVehicleId");

                    b.ToTable("UpdateRequests");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.UserPermit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Expiry")
                        .HasColumnType("datetime2");

                    b.Property<int>("PermitId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermitId");

                    b.HasIndex("UserId");

                    b.HasIndex("VehicleId");

                    b.ToTable("UserPermits", t =>
                        {
                            t.HasTrigger("LC_TRIGGER_AFTER_DELETE_USERPERMIT");

                            t.HasTrigger("LC_TRIGGER_AFTER_INSERT_USERPERMIT");

                            t.HasTrigger("LC_TRIGGER_AFTER_UPDATE_USERPERMIT");
                        });

                    b
                        .HasAnnotation("LC_TRIGGER_AFTER_DELETE_USERPERMIT", "CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT ON \"UserPermits\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldPermitId INT\r\n  DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND")
                        .HasAnnotation("LC_TRIGGER_AFTER_INSERT_USERPERMIT", "CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND")
                        .HasAnnotation("LC_TRIGGER_AFTER_UPDATE_USERPERMIT", "CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT ON \"UserPermits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @OldPermitId INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Inserted\r\n  OPEN InsertedUserPermitCursor DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldPermitId <> @NewPermitId)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationDocImgPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AccessLog", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.UserPermit", "UserPermit")
                        .WithMany("AccessLogs")
                        .HasForeignKey("UserPermitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("UserPermit");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppUserRole", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.AppUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Notification", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppUser", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Permit", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.Area", "Area")
                        .WithMany("Permits")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.PermitApplication", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.Permit", "Permit")
                        .WithMany()
                        .HasForeignKey("PermitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.AppUser", "User")
                        .WithMany("PermitApplications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.Vehicle", "Vehicle")
                        .WithMany("PermitApplications")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Permit");

                    b.Navigation("User");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.UpdateRequest", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.UserPermit", "UserPermit")
                        .WithMany("UpdateRequests")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.Permit", "NewPermit")
                        .WithMany("UpdateRequests")
                        .HasForeignKey("NewPermitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.Vehicle", "UpdatedVehicle")
                        .WithMany("UpdateRequests")
                        .HasForeignKey("UpdatedVehicleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("NewPermit");

                    b.Navigation("UpdatedVehicle");

                    b.Navigation("UserPermit");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.UserPermit", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.Permit", "Permit")
                        .WithMany("UserPermits")
                        .HasForeignKey("PermitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.AppUser", "User")
                        .WithMany("UserPermits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eCampusGuard.Core.Entities.Vehicle", "Vehicle")
                        .WithMany("UserPermits")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Permit");

                    b.Navigation("User");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Vehicle", b =>
                {
                    b.HasOne("eCampusGuard.Core.Entities.AppUser", "User")
                        .WithMany("Vehicles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppRole", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.AppUser", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("PermitApplications");

                    b.Navigation("UserPermits");

                    b.Navigation("UserRoles");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Area", b =>
                {
                    b.Navigation("Permits");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Permit", b =>
                {
                    b.Navigation("UpdateRequests");

                    b.Navigation("UserPermits");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.UserPermit", b =>
                {
                    b.Navigation("AccessLogs");

                    b.Navigation("UpdateRequests");
                });

            modelBuilder.Entity("eCampusGuard.Core.Entities.Vehicle", b =>
                {
                    b.Navigation("PermitApplications");

                    b.Navigation("UpdateRequests");

                    b.Navigation("UserPermits");
                });
#pragma warning restore 612, 618
        }
    }
}
