using System;
using Exelor.Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Exelor.Infrastructure.Auth.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(
            params Permissions[] permissions)
        {
            Policy = string.Join("|", permissions);
        }
    }
}