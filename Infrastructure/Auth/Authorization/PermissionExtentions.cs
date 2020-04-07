using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static bool ThisPermissionIsAllowed(
            this string packedPermissions,
            string permissionName)
        {
            var usersPermissions = packedPermissions.UnpackPermissions().ToArray();

            if (!Enum.TryParse(
                permissionName,
                true,
                out Permissions permissionToCheck))
                throw new InvalidEnumArgumentException(
                    $"{permissionName} could not be converted to a {nameof(Permissions)}.");

            return usersPermissions.UserHasThisPermission(permissionToCheck);
        }

        public static bool UserHasThisPermission(
            this Permissions[] usersPermissions,
            Permissions permissionToCheck)
        {
            return usersPermissions.Contains(permissionToCheck) || usersPermissions.Contains(Permissions.SuperUser);
        }
    }
}