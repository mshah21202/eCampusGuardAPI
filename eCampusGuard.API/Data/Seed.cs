using System;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.API.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRoles(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.AppRoles.CountAsync() == 0 || await unitOfWork.AppUsers.CountAsync() == 0)
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

                roles[0] = await unitOfWork.AppRoles.FindAsync(r => r.Name == roles[0].Name);
                roles[1] = await unitOfWork.AppRoles.FindAsync(r => r.Name == roles[1].Name);
                roles[2] = await unitOfWork.AppRoles.FindAsync(r => r.Name == roles[2].Name);

                var userr = new AppUser
                {
                    Name = "Mohamad Shahin",
                    UserName = "20200461"
                };

                var adminUserr = new AppUser
                {
                    Name = "Admin",
                    UserName = "21202"
                };

                var gateStaffUserr = new AppUser
                {
                    Name = "Gate Staff",
                    UserName = "20212"

                };


                await userManager.CreateAsync(userr, "$hahin1234B_");
                await userManager.AddToRoleAsync(userr, roles[0].Name);

                await userManager.CreateAsync(adminUserr, "$hahin1234B_");
                await userManager.AddToRoleAsync(adminUserr, roles[1].Name);

                await userManager.CreateAsync(gateStaffUserr, "$hahin1234B_");
                await userManager.AddToRoleAsync(gateStaffUserr, roles[2].Name);

            }
            var user = await unitOfWork.AppUsers.FindAsync(u => u.UserName == "20200461");
            var adminUser = await unitOfWork.AppUsers.FindAsync(u => u.UserName == "21202");
            var gateStaffUser = await unitOfWork.AppUsers.FindAsync(u => u.UserName == "20212");

            // Create areas
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

            if (await unitOfWork.Areas.CountAsync() == 0)
            {
                itArea = await unitOfWork.Areas.AddAsync(itArea);
                engArea = await unitOfWork.Areas.AddAsync(engArea);
                rssArea = await unitOfWork.Areas.AddAsync(rssArea);
            }

            await unitOfWork.CompleteAsync();

            // Create permits
            var permits = new List<Permit>
            {
                new Permit
                {
                    Name = "IT Daily",
                    Days = new List<bool> {true, true, true, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea
                },
                new Permit
                {
                    Name = "IT I",
                    Days = new List<bool> {true, false, true, false, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea
                },
                new Permit
                {
                    Name = "IT II",
                    Days = new List<bool> {false, true, false, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 100,
                    Area = itArea
                },
                new Permit
                {
                    Name = "Engineering Daily",
                    Days = new List<bool> {true, true, true, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 75,
                    Area = engArea
                },
                new Permit
                {
                    Name = "Engineering I",
                    Days = new List<bool> {true, false, true, false, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 90,
                    Area = engArea
                },
                new Permit
                {
                    Name = "Engineering II",
                    Days = new List<bool> {false, true, false, true, true},
                    Price = 40,
                    Occupied = 0,
                    Capacity = 150,
                    Area = engArea
                },
            };

            if (await unitOfWork.Permits.CountAsync() == 0)
            {
                permits = (await unitOfWork.Permits.AddRangeAsync(permits)).ToList();
            }

            await unitOfWork.CompleteAsync();

            permits[0] = await unitOfWork.Permits.GetByIdAsync(7);

            // Create permitApplications & vehicles

            var vehicles = new List<Vehicle>
            {
                new Vehicle
                    {
                        PlateNumber = "45-58864",
                        Nationality = "JO",
                        Make = "Hyundai",
                        Model = "Sonata",
                        Color = "White",
                        Year = 2017,
                        RegistrationDocImgPath = "",
                        User = user
                    }
            };

            var permitApplications = new List<PermitApplication>
            {
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
                new PermitApplication
                {
                    AttendingDays = new List<bool> {true, true, true, true},
                    SiblingsCount = 0,
                    Year = AcademicYear.FirstYear,
                    LicenseImgPath = "",
                    PhoneNumber = "+962799551474",
                    User = user,
                    Vehicle = vehicles[0],
                    Permit = permits[0]
                },
            };


            if (await unitOfWork.PermitApplications.CountAsync() < 10)
            {
                permitApplications = (await unitOfWork.PermitApplications.AddRangeAsync(permitApplications)).ToList();
            }

            if (await unitOfWork.CompleteAsync() < 0)
            {
                System.Console.WriteLine("Something went wrong with seeding");
            }

        }
    }
}

