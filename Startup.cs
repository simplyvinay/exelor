using System.Collections.Generic;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>();

            services.Configure<JwtSettings>(Configuration.GetSection(typeof(JwtSettings).Name));
            services.Configure<PasswordHasherSettings>(Configuration.GetSection(typeof(PasswordHasherSettings).Name));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

            services.AddControllers();
            services.AddValidationPipeline();
            services.AddCors();
            services.AddJwtAuthentication();

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
                            Title = "Exelor API", Version = "v1"
                        });
                    x.CustomSchemaIds(y => y.FullName);
                    x.DocInclusionPredicate(
                        (
                            version,
                            apiDescription) => true);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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


            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
