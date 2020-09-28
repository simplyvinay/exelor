using System.Security.Claims;
using Exelor.Infrastructure.Auth.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Exelor.Infrastructure.Auth.Authentication
{
    public interface ICurrentUser
    {
        string Id { get; }
        string Name { get; }
        string Permissions { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

        public string Id => User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                            ?? string.Empty;

        public string Name => User?.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value
                              ?? string.Empty;

        public string Permissions => User?.FindFirst(JwtRegisteredCustomClaimNames.Permissions)?.Value
                                     ?? string.Empty;
    }
}