// <copyright file="IUserIdentityActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Dapr.Actors;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Represents a Dapr actor interface for managing user identity operations.
/// </summary>
public interface IUserIdentityActor : IActor
{
    Task AddClaimsAsync(IEnumerable<Claim> claims);

    /// <summary>
    /// Creates a new user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if the creation was successful; otherwise, false.</returns>
    Task<bool> CreateAsync(UserIdentity user);

    /// <summary>
    /// Deletes a user identity asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync();

    /// <summary>
    /// Checks if a user identity exists asynchronously.
    /// </summary>
    /// <returns>True if the user identity exists; otherwise, false.</returns>
    Task<bool> ExistsAsync();

    /// <summary>
    /// Finds a user identity by its ID asynchronously.
    /// </summary>
    /// <returns>The user identity if found; otherwise, null.</returns>
    Task<UserIdentity?> FindAsync();

    Task<IList<Claim>> GetClaimsAsync();

    Task RemoveClaimsAsync(IEnumerable<Claim> claims);

    Task ReplaceClaimAsync(Claim claim1, Claim newClaim);

    /// <summary>
    /// Updates an existing user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(UserIdentity user);
}