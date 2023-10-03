using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Authorization
{
    /// <summary>
    /// Custome authorize attribute to overide default authorize behaviour
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        internal const string PolicyPrefix = "PERMISSION_";
        private const string Separator = "_";

        public PermissionAuthorizeAttribute(
            PermissionOperator permissionOperator, params string[] permissions)
        {
            // E.g: PERMISSION_1_Create_Update..
            Policy = $"{PolicyPrefix}{(int)permissionOperator}{Separator}{string.Join(Separator, permissions)}";
        }

        public PermissionAuthorizeAttribute(string permission)
        {
            // E.g: PERMISSION_1_Create..
            Policy = $"{PolicyPrefix}{(int)PermissionOperator.And}{Separator}{permission}";
        }

        /// <summary>
        /// Get the operator policy by policy name
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static PermissionOperator GetOperatorFromPolicy(string policyName)
        {
            var @operator = int.Parse(policyName.AsSpan(PolicyPrefix.Length, 1));
            return (PermissionOperator)@operator;
        }

        /// <summary>
        /// Get permission from policy
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <returns>Array of defined policy</returns>
        public static string[] GetPermissionsFromPolicy(string policyName)
        {
            return policyName.Substring(PolicyPrefix.Length + 2)
                .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
