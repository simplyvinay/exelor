using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.ErrorHandling;

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

        public static void Authorize(
            this ICurrentUser currentUser,
            params Permissions[] permissionsToCheck)
        {
            var currentUserPermissions = currentUser.Permissions.UnpackPermissions();
            var hasPermission = currentUserPermissions.Select(x => x)
                               .Intersect(permissionsToCheck)
                               .Any() || currentUserPermissions.Contains(Permissions.SuperUser);

            if(!hasPermission)
                throw new HttpException(HttpStatusCode.Forbidden);
        }
        
        public static bool IsAllowed(
            this ICurrentUser currentUser,
            params Permissions[] permissionsToCheck)
        {
            var currentUserPermissions = currentUser.Permissions.UnpackPermissions();
            return currentUserPermissions.Select(x => x)
                       .Intersect(permissionsToCheck)
                       .Any() || currentUserPermissions.Contains(Permissions.SuperUser);
        }
    }
}