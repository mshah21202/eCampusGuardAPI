using System;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using eCampusGuard.Services.AutoMapper;
using eCampusGuard.Services.TokenService;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<SQLDataContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(
                    config.GetConnectionString("SQLConnection"), b => b.MigrationsAssembly(typeof(SQLDataContext).Assembly.FullName)
                )
            );

            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<IUnitOfWork, UnitOfWorkSQL>();

            return services;
        }
    }
}

