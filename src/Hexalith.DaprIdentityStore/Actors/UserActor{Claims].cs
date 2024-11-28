// <copyright file="UserActor{Claims].cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
public partial class UserActor
{
    /// <inheritdoc/>
    public async Task AddClaimsAsync(IEnumerable<CustomUserClaim> claims)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add {nameof(claims)} failed : User '{userId}' not found.");
        }

        _state.Claims = _state.Claims.Union(claims.Where(p => p.ClaimType is not null));

        foreach (CustomUserClaim claim in claims.Where(p => p.ClaimType is not null))
        {
            await _claimIndexService.AddAsync(claim.ClaimType!, claim.ClaimValue, userId, CancellationToken.None);
        }

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CustomUserClaim>> GetClaimsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state is null
            ? throw new InvalidOperationException($"Get claims Failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Claims;
    }

    /// <summary>
    /// Removes specified claims from the user's identity.
    /// </summary>
    /// <param name="claims">Claims to remove.</param>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveClaimsAsync(IEnumerable<CustomUserClaim> claims)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove {nameof(claims)} failed : User '{userId}' not found.");
        }

        // Remove user claims
        _state.Claims = _state.Claims
            .Where(p => !claims.Any(c => c.ClaimType == p.ClaimType && c.ClaimValue == p.ClaimValue));

        foreach (CustomUserClaim claim in claims.Where(p => p.ClaimType is not null))
        {
            await _claimIndexService.RemoveAsync(claim.ClaimType!, claim.ClaimValue, userId, CancellationToken.None);
        }

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Replaces an existing claim with a new claim.
    /// Useful for updating claim values while maintaining the same claim type.
    /// </summary>
    /// <param name="claim">Existing claim to replace.</param>
    /// <param name="newClaim">New claim to add.</param>
    /// <exception cref="ArgumentNullException">When claim parameters are null.</exception>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ReplaceClaimAsync(CustomUserClaim claim, CustomUserClaim newClaim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);
        ArgumentNullException.ThrowIfNull(claim.ClaimType);
        ArgumentNullException.ThrowIfNull(newClaim.ClaimType);
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Replace {nameof(claim)} failed : User '{userId}' not found.");
        }

        // Add claims to user state and remove duplicates
        _state.Claims = _state
            .Claims
            .Where(p => p.ClaimType != claim.ClaimType || p.ClaimValue != claim.ClaimValue)
            .Union([newClaim]);

        await _claimIndexService.RemoveAsync(claim.ClaimType, claim.ClaimValue, userId, CancellationToken.None);
        await _claimIndexService.AddAsync(newClaim.ClaimType, newClaim.ClaimValue, userId, CancellationToken.None);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }
}