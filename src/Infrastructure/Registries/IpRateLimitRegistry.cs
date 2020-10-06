using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Registries
{
    public static class IpRateLimitRegistry
    {
        public static IServiceCollection AddIpRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //Can be rate limited by Client Id as well
            //ClientRateLimitOptions
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }
    }
}