using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Exelor.Infrastructure
{
    public static class WebService
    {
        public static IServiceCollection AddWeb(
            this IServiceCollection services)
        {
            //attach the the model validator and define the api grouping convention
            //setup fluent validation for the running assembly
            services.AddMvc(
                    options =>
                    {
                        options.Filters.Add<ValidateModelFilter>();
                        options.Conventions.Add(new GroupByApiRootConvention());
                    })
                .AddJsonOptions(opt => { opt.JsonSerializerOptions.IgnoreNullValues = true; })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}