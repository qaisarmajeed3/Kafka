using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Authorization
{
    /// <summary>
    /// Authorization policy permission provider to get the permission policy
    /// </summary>
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionAuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options) : base(options) { }

        /// <summary>
        /// Get the policy defined for endpoint
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <returns>Authorization policy</returns>
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(
            string policyName)
        {
            if (!policyName.StartsWith(PermissionAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
                return await base.GetPolicyAsync(policyName);

            // Will extract the Operator AND/OR enum from the string
            PermissionOperator @operator = PermissionAuthorizeAttribute.GetOperatorFromPolicy(policyName);

            // Will extract the permissions from the string (Create, Update..)
            string[] permissions = PermissionAuthorizeAttribute.GetPermissionsFromPolicy(policyName);

            // Here we create the instance of our requirement
            var requirement = new PermissionRequirement(@operator, permissions);

            // Now we use the builder to create a policy, adding our requirement
            return new AuthorizationPolicyBuilder()
                .AddRequirements(requirement).Build();
        }
    }
}
