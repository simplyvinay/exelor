using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exelor.Infrastructure.Data
{
    public static class DbRegistry
    {
        public static IServiceCollection AddSql(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                op => op.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection AddPostgres(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                op => op.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}