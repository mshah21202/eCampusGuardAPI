using System;
using eCampusGuard.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.API.Data
{
	public class Seed
	{
		public static async Task SeedUsersAndRoles(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			var roles = new List<AppRole>
			{
				new AppRole
				{
					Name = "Member",
					HomeScreenWidgets = new List<HomeScreenWidget>
					{
						HomeScreenWidget.PermitStatus
					}
				},
                new AppRole
                {
                    Name = "Admin",
                    HomeScreenWidgets = new List<HomeScreenWidget>
                    {
						HomeScreenWidget.ApplicationsSummary,
						HomeScreenWidget.AreasSummary
                    }
                },
                new AppRole
                {
                    Name = "GateStaff",
                    HomeScreenWidgets = new List<HomeScreenWidget>
                    {
                        HomeScreenWidget.AreasSummary
                    }
                },
            };

			foreach (var role in roles)
			{
				await roleManager.CreateAsync(role);
			}

			var users = new List<AppUser>
			{
				new AppUser
				{
					Name = "Member",
					UserName = "member1_"
				},
                new AppUser
                {
                    Name = "Admin",
                    UserName = "admin1_"
                },
                new AppUser
                {
                    Name = "Gate Staff",
                    UserName = "gatestaff1_"
                },
            };

			for (int i = 0; i < users.Count; i++)
			{
				await userManager.CreateAsync(users[i], users[i].UserName);
				await userManager.AddToRoleAsync(users[i], roles[i].Name);
			}
		}
	}
}

