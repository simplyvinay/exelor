using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Exelor.Infrastructure.Auth.Authorization
{
    public class AuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionsRequirement requirement)
        {
            var permissionsClaim =
                context.User.Claims.SingleOrDefault(c => c.Type == JwtRegisteredCustomClaimNames.Permissions);

            if (permissionsClaim == null)
                return Task.CompletedTask;

            if (permissionsClaim.Value.ThesePermissionsAreAllowed(requirement.Permissions))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}