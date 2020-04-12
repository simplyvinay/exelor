using AspNetCoreRateLimit;
using Exelor.Infrastructure;
using Exelor.Infrastructure.Auditing;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.ErrorHandling;
using Exelor.Infrastructure.Logging;
using Exelor.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exelor
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services)
        {
            services
                .AddMediatR(typeof(Startup))
                .AddValidationPipeline()
                .AddAudit(Configuration)
                .AddSql(Configuration)
                .AddWeb(Configuration)
                .AddSwagger() //Hook up swagger
                .AddAuthentication(Configuration)
                .AddAuth()
                .AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            //change this allow only specific origins
            app.UseCors(
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Token-Expired"));

            app.UseSwagger(
                    c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; })
                .UseSwaggerUI(
                    x =>
                    {
                        x.SwaggerEndpoint(
                            "/swagger/v1/swagger.json",
                            "Exelor API V1");
                    });

            app.UseIpRateLimiting();

            loggerFactory.AddSerilogLogging(env);
            app.UseErrorHandlingMiddleware();

            //uncomment to use caching
            //app.UseResponseCaching();
            //app.UseHttpCacheHeaders();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
