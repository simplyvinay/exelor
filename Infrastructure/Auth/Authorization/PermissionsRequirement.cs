using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Exelor.Infrastructure.Auth.Authorization
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