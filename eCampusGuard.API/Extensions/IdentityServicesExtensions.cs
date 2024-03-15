using System;
using System.Text;
using eCampusGuard.Core.Entities;
using eCampusGuard.MSSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace eCampusGuard.API.Extensions
{
	public static class IdentityServicesExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
		{
			services.AddIdentityCore<AppUser>(opt =>
			{
				opt.Password.RequireNonAlphanumeric = false;
			})
			.AddRoles<AppRole>()
			.AddRoleManager<RoleManager<AppRole>>()
			.AddSignInManager<SignInManager<AppUser>>()
			.AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<SQLDataContext>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(opt =>
				{
					opt.TokenValidationParameters = new TokenValidationParameters
					{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TOKEN_KEY"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
				});

			services.AddAuthorization(opt =>
			{
				opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("RequireGateStaffRole", policy => policy.RequireRole("Admin", "GateStaff"));
            });

			return services;
		}
	}
}

