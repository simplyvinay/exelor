using System;
using System.Collections.Generic;

namespace ApiStarter.Infrastructure.Authorization
{
    public static class PermissionPacker
    {
        public static IEnumerable<string> UnpackFromString(
            this string packedPermissions,
            string policyNameSplitBy)
        {
            if (packedPermissions == null) throw new ArgumentNullException(nameof(packedPermissions));

            return packedPermissions.Split(
                new[] {policyNameSplitBy},
                StringSplitOptions.None);
        }
    }
}