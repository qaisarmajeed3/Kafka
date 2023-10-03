using Microsoft.AspNetCore.Authorization;

namespace Kafka.Authorization
{
    /// <summary>
    /// Enum to define the And/Or operator
    /// </summary>
    public enum PermissionOperator
    {
        And = 1, Or = 2
    }
    /// <summary>
    /// Permission requirement class to manage the permission based on permission operator
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        // 1 - The operator
        public PermissionOperator PermissionOperator { get; }

        // 2 - The list of permissions passed
        public string[] Permissions { get; }

        /// <summary>
        /// Permission requirement to set the permission operator and set of permission
        /// </summary>
        /// <param name="permissionOperator">Permission operator</param>
        /// <param name="permissions">Permission array</param>
        /// <exception cref="ArgumentException"></exception>
        public PermissionRequirement(
            PermissionOperator permissionOperator, string[] permissions)
        {
            if (permissions.Length == 0)
                throw new ArgumentException("At least one permission is required.", nameof(permissions));

            PermissionOperator = permissionOperator;
            Permissions = permissions;
        }
    }
}