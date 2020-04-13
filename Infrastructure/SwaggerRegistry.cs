using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Exelor.Infrastructure
{
    public static class SwaggerRegistry 
    {
        public static IServiceCollection AddSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(
                x =>
                {
                    x.DescribeAllParametersInCamelCase();
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

            return services;
        }
    }
}