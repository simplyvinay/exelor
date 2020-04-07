using System;
using System.Collections.Generic;
using System.Linq;
using Exelor.Domain.Identity;

namespace Exelor.Infrastructure.Auth.Authorization
{
    public static class PermissionExtensions
    {
        public static string PackPermissions(
            this IEnumerable<Permissions> permissions)
        {
            return permissions.Aggregate("", (s, permission) => s + (char) permission);
        }

        public static IEnumerable<Permissions> UnpackPermissions(
            this string packedPermissions)
        {
            if (packedPermissions == null)
                throw new ArgumentNullException(nameof(packedPermissions));
            foreach (var character in packedPermissions)
            {
                yield return ((Permissions) character);
            }
        }

        public static bool ThesePermissionsAreAllowed(
            this string packedPermissions,
            IEnumerable<string> permissions)
        {
            var usersPermissions = packedPermissions.UnpackPermissions().ToArray();
            return usersPermissions.UserHasThesePermission(permissions.Select(Enum.Parse<Permissions>));
        }


        public static bool UserHasThesePermission(
            this Permissions[] usersPermissions,
            IEnumerable<Permissions> permissionsToCheck)
        {
            return usersPermissions.Select(x => x)
                .Intersect(permissionsToCheck)
                .Any() || usersPermissions.Contains(Permissions.SuperUser);
        }
    }
}