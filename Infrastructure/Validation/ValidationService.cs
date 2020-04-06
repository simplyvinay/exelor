using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Exelor.Infrastructure.Validation
{
    public static class ValidationService
    {
        public static void AddValidationPipeline(
            this IServiceCollection services)
        {
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>));
        }
    }
}