using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exelor.Infrastructure.Auditing
{
    public static class AuditRegistry
    {
        public static IServiceCollection AddAudit(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AuditSettings>(configuration.GetSection(typeof(AuditSettings).Name));
            return services;
        }
    }
}