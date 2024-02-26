using System;
using eCampusGuard.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.MSSQL
{
	public class SQLDataContext : DbContext
	{
		// Create DbSets (Tables) for entities
		public DbSet<User> Users { get; set; }
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

        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.Entity<User>().Property(u => u.Id).ValueGeneratedNever();

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

