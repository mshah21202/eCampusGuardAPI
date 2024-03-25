using System;
using System.Collections;
using System.Reflection.Emit;
using eCampusGuard.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace eCampusGuard.MSSQL
{
	public class SQLDataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
		// Create DbSets (Tables) for entities
		public DbSet<AppUser> AppUsers { get; set; }
		public DbSet<Permit> Permits { get; set; }
		public DbSet<Area> Areas { get; set; }
		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<UserPermit> UserPermits { get; set; }
		public DbSet<PermitApplication> PermitApplications { get; set; }
		public DbSet<AccessLog> AccessLogs { get; set; }

		public SQLDataContext()
		{

		}

		public SQLDataContext(DbContextOptions<SQLDataContext> options) : base(options)
		{
			
		}

        private int getIntFromBitArray(BitArray bitArray)
        {
            int value = 0;

            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i])
                    value += Convert.ToInt16(Math.Pow(2, i));
            }

            return value;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.Entity<AppUser>().Property(u => u.Id).ValueGeneratedNever();

            builder.Entity<Permit>().Property(p => p.Days).HasColumnType("int").HasConversion(v => getIntFromBitArray(v), v => new BitArray(v, false));
			builder.Entity<PermitApplication>().Property(p => p.AttendingDays).HasColumnType("int").HasConversion(v => getIntFromBitArray(v), v => new BitArray(v, false));

            builder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.AppUser)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.AppRole)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            var homeScreenWidgetsValueComparer = new ValueComparer<IEnumerable<HomeScreenWidget>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToHashSet());

            builder.Entity<AppRole>()
                .Property(r => r.HomeScreenWidgets)
                .HasColumnType("nvarchar")
                .HasConversion(w => string.Join(',', w),
                ws => ws.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(wss => (HomeScreenWidget)int.Parse(wss)))
                .Metadata
                .SetValueComparer(homeScreenWidgetsValueComparer);

            builder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();



            builder.Entity<UserPermit>()
				.HasKey(up => new { up.UserId, up.PermitId });
			builder.Entity<UserPermit>()
				.HasOne(up => up.Permit)
				.WithMany(p => p.UserPermits)
				.HasForeignKey(up => up.PermitId);
			builder.Entity<UserPermit>()
				.HasOne(up => up.User)
				.WithMany(u => u.UserPermits)
				.HasForeignKey(up => up.UserId);
            builder.Entity<UserPermit>()
                .HasOne(up => up.Vehicle)
                .WithMany(v => v.UserPermits)
				.HasForeignKey(up => up.VehicleId)
				.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AccessLog>()
				.HasOne(al => al.User)
				.WithMany(u => u.AccessLogs)
				.HasForeignKey(al => al.UserId)
				.OnDelete(DeleteBehavior.NoAction);
            builder.Entity<AccessLog>()
                .HasOne(al => al.Vehicle)
                .WithMany(v => v.AccessLogs)
                .HasForeignKey(al => al.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<AccessLog>()
                .HasOne(al => al.Permit)
                .WithMany(p => p.AccessLogs)
                .HasForeignKey(al => al.PermitId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PermitApplication>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.PermitApplications)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.NoAction);
           

            base.OnModelCreating(builder);
        }
    }
}

