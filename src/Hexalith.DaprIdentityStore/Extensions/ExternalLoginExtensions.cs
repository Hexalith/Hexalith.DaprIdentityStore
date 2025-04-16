// <copyright file="ExternalLoginExtensions.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Extensions;

using System.Security.Claims;
using System.Threading.Tasks;
using Hexalith.DaprIdentityStore.Models;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Extension methods for handling external logins and identity providers.
/// </summary>
public static class ExternalLoginExtensions
{
    /// <summary>
    /// Synchronizes roles from the external identity provider with the local user.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="user">The user to update roles for.</param>
    /// <param name="externalLoginInfo">The external login information containing roles as claims.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SyncExternalProviderRolesAsync(
        this UserManager<CustomUser> userManager,
        CustomUser user,
        ExternalLoginInfo externalLoginInfo)
    {
        if (userManager == null || user == null || externalLoginInfo == null)
        {
            return;
        }

        // Get all role claims from the external provider
        var externalRoleClaims = externalLoginInfo.Principal.FindAll(claim => 
            claim.Type == ClaimTypes.Role || 
            claim.Type == "role" || // Common claim type in some providers
            claim.Type == "roles" || // Some providers use this format
            claim.Type.EndsWith("/roles")); // Some providers use URIs for claim types

        if (!externalRoleClaims.Any())
        {
            return;
        }

        // Get current user roles
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = new List<string>();

        // Extract role values from the claims
        foreach (var roleClaim in externalRoleClaims)
        {
            string role = roleClaim.Value;
            if (!string.IsNullOrWhiteSpace(role) && !currentRoles.Contains(role))
            {
                rolesToAdd.Add(role);
            }
        }

        // Add the new roles to the user
        if (rolesToAdd.Any())
        {
            await userManager.AddToRolesAsync(user, rolesToAdd);
        }
    }
} 