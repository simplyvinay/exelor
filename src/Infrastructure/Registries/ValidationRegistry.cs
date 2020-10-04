using Infrastructure.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Registries
{
    public static class ValidationRegistry
    {
        //hook up validation into MediatR pipeline
        public static IServiceCollection AddValidationPipeline(
            this IServiceCollection services)
        {
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>));
            return services;
        }
    }
}