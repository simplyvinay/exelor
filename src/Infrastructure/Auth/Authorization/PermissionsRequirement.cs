using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Auth.Authorization
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public IEnumerable<string> Permissions { get; }

        public PermissionsRequirement(
            IEnumerable<string> permissions)
        {
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }
    }
}