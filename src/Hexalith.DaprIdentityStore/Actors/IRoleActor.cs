// <copyright file="IRoleActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Dapr.Actors;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Represents a Dapr actor interface for managing role identity operations.
/// </summary>
public interface IRoleActor : IActor
{
    /// <summary>
    /// Adds a collection of claims to the role identity asynchronously.
    /// </summary>
    /// <param name="claims">The collection of claims to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddClaimsAsync(IEnumerable<Claim> claims);

    /// <summary>
    /// Creates a new role identity asynchronously.
    /// </summary>
    /// <param name="role">The role identity to create.</param>
    /// <returns>True if the creation was successful; otherwise, false.</returns>
    Task<bool> CreateAsync(CustomRole role);

    /// <summary>
    /// Deletes a role identity asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync();

    /// <summary>
    /// Checks if a role identity exists asynchronously.
    /// </summary>
    /// <returns>True if the role identity exists; otherwise, false.</returns>
    Task<bool> ExistsAsync();

    /// <summary>
    /// Finds a role identity by its ID asynchronously.
    /// </summary>
    /// <returns>The role identity if found; otherwise, null.</returns>
    Task<CustomRole?> FindAsync();

    /// <summary>
    /// Retrieves all claims associated with the role identity asynchronously.
    /// </summary>
    /// <returns>A collection of claims associated with the role.</returns>
    Task<IEnumerable<Claim>> GetClaimsAsync();

    /// <summary>
    /// Removes a collection of claims from the role identity asynchronously.
    /// </summary>
    /// <param name="claims">The collection of claims to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveClaimsAsync(IEnumerable<Claim> claims);

    /// <summary>
    /// Replaces an existing claim with a new claim asynchronously.
    /// </summary>
    /// <param name="claim">The existing claim to replace.</param>
    /// <param name="newClaim">The new claim that will replace the existing claim.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReplaceClaimAsync(Claim claim, Claim newClaim);

    /// <summary>
    /// Updates an existing role identity asynchronously.
    /// </summary>
    /// <param name="role">The role identity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(CustomRole role);
}