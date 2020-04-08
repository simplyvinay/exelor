using System.Collections.Generic;
using Exelor.Infrastructure;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.ErrorHandling;
using Exelor.Infrastructure.Logging;
using Exelor.Infrastructure.Validation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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
            services.AddMediatR(typeof(Startup));

            //hook up validation into MediatR pipeline
            services.AddValidationPipeline();

            services.AddDbContext<ApplicationDbContext>();

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

            //Hook up swagger
            services.AddSwaggerGen(
                x =>
                {
                    x.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "bearer"
                        });

                    var requirement = new OpenApiSecurityRequirement();
                    requirement.Add(
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>());
                    x.AddSecurityRequirement(
                        requirement);
                    x.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Exelor API",
                            Version = "v1"
                        });
                    x.CustomSchemaIds(y => y.FullName);
                    x.DocInclusionPredicate(
                        (
                            version,
                            apiDescription) => true);
                });

            services.Configure<JwtSettings>(Configuration.GetSection(typeof(JwtSettings).Name));
            services.Configure<PasswordHasherSettings>(Configuration.GetSection(typeof(PasswordHasherSettings).Name));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

            services.AddControllers(
                config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                });
            services.AddJwtAuthentication();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilogLogging();
            app.UseErrorHandlingMiddleware();
            
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
                c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                });

            app.UseSwaggerUI(
                x =>
                {
                    x.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "Exelor API V1");
                });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
