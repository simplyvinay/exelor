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

        public string Id => _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                            string.Empty;

        public string Name =>
            _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;

        public string Permissions =>
            _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredCustomClaimNames.Permissions)?.Value ??
            string.Empty;
    }
}