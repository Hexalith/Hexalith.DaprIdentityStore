// <copyright file="IdentityRevalidatingAuthenticationStateProvider.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Account;

using System.Security.Claims;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// A server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
/// every 30 minutes while an interactive circuit is connected.
/// </summary>
/// <param name="loggerFactory">The logger factory to create logger instances.</param>
/// <param name="scopeFactory">The service scope factory to create new service scopes.</param>
/// <param name="options">The identity options to configure identity settings.</param>
public sealed class IdentityRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<IdentityOptions> options)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    /// <inheritdoc/>
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(authenticationState);

        // Get the user manager from a new scope to ensure it fetches fresh data
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        UserManager<CustomUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<CustomUser> userManager, ClaimsPrincipal principal)
    {
        CustomUser? user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            string? principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
            string userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }
}