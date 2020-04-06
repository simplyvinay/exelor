using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ApiStarter.Infrastructure.Auth.Authorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public AuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(
            string policyName)
        {
            AuthorizationPolicy policy = null;
            if (!policyName.StartsWith(
                PermissionConstant.PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
            {
                policy = await base.GetPolicyAsync(policyName);
            }

            if (policy == null)
            {

                /*var permissions = policyName.Substring(PermissionConstant.PolicyPrefix.Length)
                    .UnpackFromString(PermissionConstant.PolicyNameSplitBy);

                return new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permissions))
                    .Build();*/
            }

            return policy;
        }
    }
}