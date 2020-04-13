using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;

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

            ConfigureSieve(
                services,
                configuration);
            return services;
        }

        public static IServiceCollection AddPostgres(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                op => op.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            ConfigureSieve(
                services,
                configuration);
            return services;
        }

        private static void ConfigureSieve(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SieveOptions>(configuration.GetSection("Sieve"));
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
        }
    }
}