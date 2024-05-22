using System;
using System.Text.Json;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.API.Data
{
    public class Seed
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;


        public Seed(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        private async Task SeedRoles()
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
                await _roleManager.CreateAsync(role);
            }
        }

        private List<AppUser> GetUsersFromJson()
        {
            var jsonText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data/mock_users.json"));

            List<Dictionary<string, string>> usersDict = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonText, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            List<AppUser> users = new();

            foreach (var user in usersDict)
            {
                users.Add(new AppUser
                {
                    Name = user["name"],
                    UserName = user["userName"],
                    Email = "mohamad.shahin2002@gmail.com",
                    EmailConfirmed = true,
                });
            }

            return users;
        }

        private async Task SeedUsers()
        {
            try
            {
                var users = GetUsersFromJson();

                foreach (var user in users)
                {
                    var userr = new AppUser
                    {
                        Name = user.Name,
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed
                    };
                    await _userManager.CreateAsync(userr, "$hahin1234B_");
                    await _userManager.AddToRoleAsync(userr, "Member");
                }

                // Admin and gate staff
                var adminUser = new AppUser
                {
                    Name = "Admin",
                    UserName = "21202"
                };

                var gateStaffUser = new AppUser
                {
                    Name = "Gate Staff",
                    UserName = "20212"

                };

                await _userManager.CreateAsync(adminUser, "$hahin1234B_");
                await _userManager.AddToRoleAsync(adminUser, "Admin");


                await _userManager.CreateAsync(gateStaffUser, "$hahin1234B_");
                await _userManager.AddToRoleAsync(gateStaffUser, "GateStaff");

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        private async Task SeedAreas()
        {
            var itArea = new Area
            {
                Name = "IT",
                Gate = "IT Gate",
                Occupied = 0,
                Capacity = 250
            };

            var engArea = new Area
            {
                Name = "Engineering",
                Gate = "Engineering Gate",
                Occupied = 0,
                Capacity = 450
            };

            var rssArea = new Area
            {
                Name = "RSS",
                Gate = "RSS Gate",
                Occupied = 0,
                Capacity = 200
            };

            itArea = await _unitOfWork.Areas.AddAsync(itArea);
            engArea = await _unitOfWork.Areas.AddAsync(engArea);
            rssArea = await _unitOfWork.Areas.AddAsync(rssArea);

            if (await _unitOfWork.CompleteAsync() == 0)
            {
                System.Console.WriteLine("Something went wrong with seeding areas");
            }
        }

        private async Task SeedPermits()
        {
            var itArea = await _unitOfWork.Areas.FindAsync((a) => a.Name == "IT");
            var engArea = await _unitOfWork.Areas.FindAsync((a) => a.Name == "Engineering");
            var rssArea = await _unitOfWork.Areas.FindAsync((a) => a.Name == "RSS");

            var permits = new List<Permit>
            {
                new Permit
                {
                    Name = "IT Daily",
                    Days = new List<bool> {true, true, true, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
                new Permit
                {
                    Name = "IT I",
                    Days = new List<bool> {true, false, true, false, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
                new Permit
                {
                    Name = "IT II",
                    Days = new List<bool> {false, true, false, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
                new Permit
                {
                    Name = "Engineering Daily",
                    Days = new List<bool> {true, true, true, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 75,
                    Area = engArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
                new Permit
                {
                    Name = "Engineering I",
                    Days = new List<bool> {true, false, true, false, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 90,
                    Area = engArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
                new Permit
                {
                    Name = "Engineering II",
                    Days = new List<bool> {false, true, false, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 150,
                    Area = engArea,
                    Expiry = DateTime.Now.AddDays(7)
                },
            };

            (await _unitOfWork.Permits.AddRangeAsync(permits)).ToList();

            if (await _unitOfWork.CompleteAsync() == 0)
            {
                System.Console.WriteLine("Something went wrong with seeding permits");
            }
        }

        private List<Vehicle> GetVehiclesFromJson()
        {
            var jsonText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data/mock_vehicles.json"));

            List<Vehicle> vehicles = JsonSerializer.Deserialize<List<Vehicle>>(jsonText, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            return vehicles;
        }

        private async Task SeedVehicles()
        {
            var vehicles = GetVehiclesFromJson();

            foreach (var vehicle in vehicles)
            {
                vehicle.User = await _unitOfWork.AppUsers.GetByIdAsync(vehicle.UserId);
                await _unitOfWork.Vehicles.AddAsync(vehicle);
            }

            if (await _unitOfWork.CompleteAsync() == 0)
            {
                System.Console.WriteLine("Something went wrong with seeding vehicles");
            }
        }

        private List<PermitApplication> GetApplicationsFromJson()
        {
            var jsonText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "data/mock_applications.json"));

            List<PermitApplication> applications = JsonSerializer.Deserialize<List<PermitApplication>>(jsonText, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            return applications;
        }

        private async Task SeedApplications()
        {
            var applications = GetApplicationsFromJson();

            foreach (var application in applications)
            {
                application.User = await _unitOfWork.AppUsers.GetByIdAsync(application.UserId);
                application.Vehicle = await _unitOfWork.Vehicles.GetByIdAsync(application.VehicleId);
                application.Permit = await _unitOfWork.Permits.GetByIdAsync(application.PermitId);

                await _unitOfWork.PermitApplications.AddAsync(application);
            }

            if (await _unitOfWork.CompleteAsync() == 0)
            {
                System.Console.WriteLine("Something went wrong with seeding vehicles");
            }
        }

        public async Task SeedData()
        {
            if (await _unitOfWork.AppRoles.CountAsync() < 3)
            {
                await SeedRoles();
            }

            if (await _unitOfWork.AppUsers.CountAsync() < 20)
            {
                await SeedUsers();
            }

            if (await _unitOfWork.Areas.CountAsync() < 3)
            {
                await SeedAreas();
            }

            if (await _unitOfWork.Permits.CountAsync() < 6)
            {
                await SeedPermits();
            }

            if (await _unitOfWork.Vehicles.CountAsync() < 20)
            {
                await SeedVehicles();
            }

            if (await _unitOfWork.PermitApplications.CountAsync() < 5)
            {
                await SeedApplications();
            }
        }
    }
}

