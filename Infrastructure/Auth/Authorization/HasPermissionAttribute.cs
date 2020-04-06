using System;
using ApiStarter.Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ApiStarter.Infrastructure.Auth.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permissions permission) : base(permission.ToString())
        { }
    }
}