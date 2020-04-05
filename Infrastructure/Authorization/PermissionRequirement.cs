using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ApiStarter.Infrastructure.Authorization
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