// <copyright file="RoleActor{Claims].cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing role identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for role identities and maintains associated indexes.
/// </summary>
public partial class RoleActor
{
    /// <summary>
    /// Adds claims to the role identity.
    /// Claims are unique combinations of ClaimType and ClaimValue associated with a role.
    /// </summary>
    /// <param name="claims">Collection of claims to add to the role.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the role state is not found.</exception>
    public async Task AddClaimsAsync(IEnumerable<Claim> claims)
    {
        string roleId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add {nameof(claims)} failed : Role '{roleId}' not found.");
        }

        // Add claims to role state and remove duplicates
        IEnumerable<CustomRoleClaim> newClaims = claims
            .Select(p => new CustomRoleClaim { RoleId = roleId, ClaimType = p.Type, ClaimValue = p.Value });
        _state.Claims = _state.Claims.Union(newClaims);

        foreach (Claim claim in claims)
        {
            await _claimIndexService.AddAsync(claim, roleId, CancellationToken.None);
        }

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.RoleStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Retrieves all claims associated with the role.
    /// Claims represent role attributes and permissions.
    /// </summary>
    /// <returns>Collection of role claims.</returns>
    /// <exception cref="InvalidOperationException">When role not found.</exception>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state is null
            ? throw new InvalidOperationException($"Get claims Failed : Role '{Id.ToUnescapeString()}' not found.")
            : _state.Claims
            .Select(c => new Claim(c.ClaimType ?? string.Empty, c.ClaimValue ?? string.Empty));
    }

    /// <summary>
    /// Removes specified claims from the role's identity.
    /// </summary>
    /// <param name="claims">Claims to remove.</param>
    /// <exception cref="InvalidOperationException">When role not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveClaimsAsync(IEnumerable<Claim> claims)
    {
        string roleId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove {nameof(claims)} failed : Role '{roleId}' not found.");
        }

        // Remove role claims
        _state.Claims = _state.Claims
            .Where(p => !claims.Any(c => c.Type == p.ClaimType && c.Value == p.ClaimValue));

        foreach (Claim claim in claims)
        {
            await _claimIndexService.RemoveAsync(claim, roleId, CancellationToken.None);
        }

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.RoleStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Replaces an existing claim with a new claim.
    /// Useful for updating claim values while maintaining the same claim type.
    /// </summary>
    /// <param name="claim">Existing claim to replace.</param>
    /// <param name="newClaim">New claim to add.</param>
    /// <exception cref="ArgumentNullException">When claim parameters are null.</exception>
    /// <exception cref="InvalidOperationException">When role not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ReplaceClaimAsync(Claim claim, Claim newClaim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);
        string roleId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Replace {nameof(claim)} failed : Role '{roleId}' not found.");
        }

        // Add claims to role state and remove duplicates
        _state.Claims = _state
            .Claims
            .Where(p => p.ClaimType != claim.Type || p.ClaimValue != claim.Value)
            .Union([new CustomRoleClaim
            {
                RoleId = Id.ToUnescapeString(),
                ClaimType = newClaim.ValueType,
                ClaimValue = newClaim.Value,
            }
            ]);

        await _claimIndexService.RemoveAsync(claim, roleId, CancellationToken.None);
        await _claimIndexService.AddAsync(newClaim, roleId, CancellationToken.None);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.RoleStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }
}