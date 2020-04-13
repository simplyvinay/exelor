using AspNetCoreRateLimit;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exelor.Infrastructure
{
    public static class WebRegistry
    {
        public static IServiceCollection AddWeb(
            this IServiceCollection services,
            IConfiguration configuration)
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

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(
                implementationFactory =>
                {
                    var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                    return new UrlHelper(actionContext);
                });

            services.AddHttpCacheHeaders(
                expirationModelOptionsAction => { expirationModelOptionsAction.MaxAge = 120; },
                validationModelOptionsAction => { validationModelOptionsAction.MustRevalidate = true; });
            services.AddResponseCaching();

            services.AddMemoryCache();
            
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