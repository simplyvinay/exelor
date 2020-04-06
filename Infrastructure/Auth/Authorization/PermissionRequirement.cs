using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ApiStarter.Infrastructure.Auth.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public IEnumerable<string> Permissions { get; }

        public PermissionRequirement(
            IEnumerable<string> permissions)
        {
            Permissions = permissions;
        }
    }
}