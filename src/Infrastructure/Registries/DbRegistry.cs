using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Registries
{
    public static class DbRegistry
    {
        public static IServiceCollection AddPostgres(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                op => op
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .UseLowerCaseNamingConvention());
            return services;
        }
}