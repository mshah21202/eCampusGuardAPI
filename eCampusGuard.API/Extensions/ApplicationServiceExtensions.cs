using System;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using eCampusGuard.Services.AutoMapper;
using eCampusGuard.Services.NotificationServices;
using eCampusGuard.Services.TokenService;
using Laraue.EfCoreTriggers.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using SendGrid;

namespace eCampusGuard.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            
            services.AddDbContext<SQLDataContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(
                    config.GetConnectionString("SQLConnection"), b => b.MigrationsAssembly(typeof(SQLDataContext).Assembly.FullName)
                ).UseSqlServerTriggers();
            }
            );

            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddScoped<INotificationService<Response>, EmailNotificationService>();

            services.AddScoped<IUnitOfWork, UnitOfWorkSQL>();

            return services;
        }
    }
}

