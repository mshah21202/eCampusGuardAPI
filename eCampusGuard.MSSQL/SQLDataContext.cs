using System;
using System.Collections;
using System.Reflection.Emit;
using eCampusGuard.Core.Entities;
using Laraue.EfCoreTriggers.Common.Extensions;
//using Laraue.EfCoreTriggers.Common.TriggerBuilders.Base;
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
        public DbSet<UpdateRequest> UpdateRequests { get; set; }

        public SQLDataContext()
        {

        }

        public SQLDataContext(DbContextOptions<SQLDataContext> options) : base(options)
        {
        }

        private static int GetIntFromBitArray(IList<bool> bitArray)
        {
            int value = 0;

            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i])
                    value += Convert.ToInt16(Math.Pow(2, i));
            }
            return value;
        }

        private static IList<bool> GetBitArrayFromInt(int value)
        {
            var result = new List<bool> { false, false, false, false, false };
            var j = 0;
            for (int i = result.Count - 1; i >= 0; i--)
            {
                var weight = Convert.ToInt16(Math.Pow(2, i));

                if (value >= weight)
                {
                    value -= weight;
                    result[j] = true;
                }

                j++;
            }

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<AppUser>().Property(u => u.Id).ValueGeneratedNever();

            var boolArrayComparer = new ValueComparer<IList<bool>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray());

            builder.Entity<Permit>()
                .Property(p => p.Days)
                .HasColumnType("int")
                .HasConversion(v => GetIntFromBitArray(v), v => GetBitArrayFromInt(v))
                .Metadata
                .SetValueComparer(boolArrayComparer);

            builder.Entity<PermitApplication>()
                .Property(p => p.AttendingDays)
                .HasColumnType("int")
                .HasConversion(v => GetIntFromBitArray(v), v => GetBitArrayFromInt(v))
                .Metadata
                .SetValueComparer(boolArrayComparer);

            builder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppRole>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction);


            var homeScreenWidgetsValueComparer = new ValueComparer<IEnumerable<HomeScreenWidget>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToHashSet());

            builder.Entity<AppRole>()
                .Property(r => r.HomeScreenWidgets)
                .HasColumnType("nvarchar(max)")
                .HasConversion(w => string.Join(",", w.Select(x => (int)x)),
                ws => ws.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(wss => (HomeScreenWidget)int.Parse(wss)))
                .Metadata
                .SetValueComparer(homeScreenWidgetsValueComparer);

            builder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();



            //        builder.Entity<UserPermit>()
            //.HasKey(up => new { up.UserId, up.PermitId });
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
            builder.Entity<UserPermit>()
                .HasOne(up => up.PermitApplication)
                .WithOne(pa => pa.UserPermit)
                .HasForeignKey<UserPermit>(up => up.PermitApplicationId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<AccessLog>()
                .HasOne(al => al.UserPermit)
                .WithMany(u => u.AccessLogs)
                .HasForeignKey(al => al.UserPermitId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AccessLog>()
                .HasOne(al => al.Area)
                .WithMany(u => u.AccessLogs)
                .HasForeignKey(al => al.AreaId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<PermitApplication>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.PermitApplications)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PermitApplication>()
                .HasOne(pa => pa.Vehicle)
                .WithMany(v => v.PermitApplications)
                .HasForeignKey(pa => pa.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PermitApplication>()
                .HasOne(pa => pa.UserPermit)
                .WithOne(up => up.PermitApplication)
                .HasForeignKey<PermitApplication>(pa => pa.UserPermitId)
                //.IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UpdateRequest>()
                .HasOne(ur => ur.UserPermit)
                .WithMany(up => up.UpdateRequests)
                .HasForeignKey(ur => ur.UserPermitId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UpdateRequest>()
                .HasOne(ur => ur.UpdatedVehicle)
                .WithMany(p => p.UpdateRequests)
                .HasForeignKey(ur => ur.UpdatedVehicleId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<UserPermit>()
                .AfterUpdate(trigger => trigger
                .Action(
                    action => action
                        .Condition((before, after) => before.PermitId != after.PermitId)
                        .ExecuteRawSql("UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = {0} AND [Status] = 0) WHERE dbo.Permits.Id = {0}", (before, after) => after.PermitId)
                        .ExecuteRawSql("UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = {0} AND [Status] = 0) WHERE dbo.Permits.Id = {0}", (before, after) => before.PermitId)
                    )
                )
                .AfterInsert(trigger => trigger
                .Action(action => action
                    .Condition(ur => ur.Status == UserPermitStatus.Valid)
                    .ExecuteRawSql("UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = {0} AND [Status] = 0) WHERE dbo.Permits.Id = {0}", ur => ur.PermitId)
                   )
                )
                .AfterDelete(trigger => trigger
                .Action(action => action
                    .ExecuteRawSql("UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = {0} AND [Status] = 0) WHERE dbo.Permits.Id = {0}", ur => ur.PermitId)
                ));

            builder.Entity<Permit>()
                .AfterUpdate(trigger => trigger
                .Action(
                    action => action
                        .ExecuteRawSql("UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = {0}) WHERE dbo.Areas.Id = {1}", (before, after) => after.Id, (before, after) => after.AreaId)
                        .ExecuteRawSql("UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = {0}) WHERE dbo.Areas.Id = {1}", (before, after) => before.Id, (before, after) => before.AreaId)
                    )
                );

            builder.Entity<AccessLog>()
                .AfterInsert(trigger =>
                    trigger.Action(action =>
                        action.ExecuteRawSql("UPDATE dbo.Areas SET CurrentOccupied = dbo.CURR_OCCUPIED({0}) WHERE dbo.Areas.Id = {0}", (al) => al.AreaId)));
        }
    }
}

