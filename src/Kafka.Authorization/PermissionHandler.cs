using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Kafka.Authorization
{
    /// <summary>
    /// Permission handler to get assigned permission from ignite cache
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected IHttpContextAccessor? _httpContextAccessor;
        private readonly ILogger<PermissionHandler> _logger;
        private string _activeDirectoryId;
        private readonly IConfiguration _configuration;
        public PermissionHandler(IHttpContextAccessor accessor, ILogger<PermissionHandler> logger, IConfiguration configuration)
        {
            _httpContextAccessor = accessor;
            _logger = logger;
            _configuration = configuration;
            _activeDirectoryId = GetLoggedInUserId();
        }
        /// <summary>
        /// Handle the requirement based on assigned permission
        /// </summary>
        /// <param name="context">Http context</param>
        /// <param name="requirement">Permission requirement</param>
        /// <returns>Retrun the task</returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            string securityEnabled = _configuration["EnableBackendSecurity"];
            bool isBackendSecuirtyEnabled = string.IsNullOrEmpty(securityEnabled) ? true : bool.Parse(securityEnabled);
            if (!isBackendSecuirtyEnabled)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            string[] permissionList = GetCurrentUserPermissions();
            if (requirement.PermissionOperator == PermissionOperator.And)
            {
                foreach (var permission in requirement.Permissions)
                {
                    if (!permissionList.Contains(permission))
                    {
                        // If the user lacks ANY of the required permissions
                        // we mark it as failed.
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                // identity has all required permissions
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            foreach (var permission in requirement.Permissions)
            {
                if (permissionList.Contains(permission))
                {
                    // In the OR case, as soon as we found a matching permission
                    // we can already mark it as Succeed
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // identity does not have any of the required permissions
            context.Fail();
            return Task.CompletedTask;
        }
        public string[] GetCurrentUserPermissions()
        {
            var cacheEndpoint = _configuration["ApacheCache:Endpoint"];
            IgniteClientConfiguration clientConfiguration = new IgniteClientConfiguration
            {
                Endpoints = new List<string>
                    {
                        cacheEndpoint
                    },

            };
            IIgniteClient igniteClient = Ignition.StartClient(clientConfiguration);
            ICacheClient<string, string[]> cacheClient = igniteClient.GetOrCreateCache<string, string[]>("userSession");
            return cacheClient.Get(_activeDirectoryId);
        }

        /// <summary>
        /// Read token from header.
        /// </summary>
        /// <returns></returns>
        private string GetLoggedInUserId()
        {
            string token = string.Empty;
            JwtSecurityToken? securityToken = null;
            if (_httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers != null &&
                _httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationKey = _httpContextAccessor.HttpContext.Request.Headers.Where(x => !string.IsNullOrEmpty(x.Key) && x.Key.Equals("Authorization"));
                if (authorizationKey != null)
                {
                    var authorizationValue = authorizationKey.Select(x => x.Value.FirstOrDefault());
                    if (authorizationValue != null)
                    {
                        var tokenString = authorizationValue.FirstOrDefault();
                        if (tokenString != null)
                        {
                            token = tokenString.Split(" ")[1];
                        }
                    }
                }
                if (token != null && token != "null")
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                }
                string? stringClaimValue = string.Empty;
                if (securityToken?.Claims != null)
                {
                    var securityClaims = securityToken.Claims.FirstOrDefault(claim => claim.Type == "oid");
                    if (securityClaims != null)
                    {
                        stringClaimValue = securityClaims.Value;
                    }
                }

                return stringClaimValue;
            }
            return null;
        }
    }
}
