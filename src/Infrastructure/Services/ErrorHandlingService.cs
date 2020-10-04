using Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Services
{
    public static class ErrorHandlingService
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}