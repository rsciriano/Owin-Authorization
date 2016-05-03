// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Authorization.Infrastructure;

namespace Microsoft.Owin.Security.Authorization
{
    public static class AuthorizationServiceExtensions
    {
        public static async Task<bool> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, IAuthorizeData authorizeData)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (authorizeData == null)
            {
                throw new ArgumentNullException(nameof(authorizeData));
            }

            if (!string.IsNullOrEmpty(authorizeData.Policy))
            {
                if (!await service.AuthorizeAsync(user, authorizeData.Policy))
                {
                    return false;
                }
            }

            var builder = new AuthorizationPolicyBuilder();
            if (!string.IsNullOrEmpty(authorizeData.ActiveAuthenticationSchemes))
            {
                builder.AddAuthenticationSchemes(SplitAndTrim(authorizeData.ActiveAuthenticationSchemes));
            }

            if (!string.IsNullOrWhiteSpace(authorizeData.Roles))
            {
                builder.RequireRole(SplitAndTrim(authorizeData.Roles));
            }
            // todo: decide if authenticated user is required for resource authorization
            //else
            //{
            //    builder.RequireAuthenticatedUser();
            //}

            if (builder.Requirements.Count > 0)
            {
                var policy = builder.Build();
                return await service.AuthorizeAsync(user, policy);
            }

            return true;
        }

        private static string[] SplitAndTrim(string commaSeparated)
        {
            var split = commaSeparated.Split(',');
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }

            return split;
        }

        /// <summary>
        /// Checks if a user meets a specific requirement for the specified resource
        /// </summary>
        /// <param name="service">The <see cref="IAuthorizationService"/>.</param>
        /// <param name="user"></param>
        /// <param name="resource"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public static Task<bool> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, object resource, IAuthorizationRequirement requirement)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return service.AuthorizeAsync(user, resource, new IAuthorizationRequirement[] { requirement });
        }

        /// <summary>
        /// Checks if a user meets a specific authorization policy
        /// </summary>
        /// <param name="service">The authorization service.</param>
        /// <param name="user">The user to check the policy against.</param>
        /// <param name="resource">The resource the policy should be checked with.</param>
        /// <param name="policy">The policy to check against a specific context.</param>
        /// <returns><value>true</value> when the user fulfills the policy, <value>false</value> otherwise.</returns>
        public static Task<bool> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, object resource, AuthorizationPolicy policy)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            return service.AuthorizeAsync(user, resource, policy.Requirements.ToArray());
        }

        /// <summary>
        /// Checks if a user meets a specific authorization policy
        /// </summary>
        /// <param name="service">The authorization service.</param>
        /// <param name="user">The user to check the policy against.</param>
        /// <param name="policy">The policy to check against a specific context.</param>
        /// <returns><value>true</value> when the user fulfills the policy, <value>false</value> otherwise.</returns>
        public static Task<bool> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, AuthorizationPolicy policy)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            return service.AuthorizeAsync(user, resource: null, policy: policy);
        }

        /// <summary>
        /// Checks if a user meets a specific authorization policy
        /// </summary>
        /// <param name="service">The authorization service.</param>
        /// <param name="user">The user to check the policy against.</param>
        /// <param name="policyName">The name of the policy to check against a specific context.</param>
        /// <returns><value>true</value> when the user fulfills the policy, <value>false</value> otherwise.</returns>
        public static Task<bool> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, string policyName)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            return service.AuthorizeAsync(user, resource: null, policyName: policyName);
        }
    }
}