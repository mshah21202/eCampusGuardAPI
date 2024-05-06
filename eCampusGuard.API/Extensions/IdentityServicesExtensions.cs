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

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("RequireGateStaffRole", policy => policy.RequireRole("Admin", "GateStaff"));
            });

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
					opt.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var path = context.Request.Path;
							if (path.StartsWithSegments("/Area/feed"))
							{
								var accessToken = context.Request.Query["access_token"];
								if (!string.IsNullOrEmpty(accessToken))
								{
									context.Request.Headers.Add("Authorization", new[] { $"Bearer {accessToken}" });
								}
							}
							return Task.CompletedTask;
						}
					};
				});
			

			return services;
		}
	}
}

