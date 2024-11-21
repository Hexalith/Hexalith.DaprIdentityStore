// <copyright file="IRoleClaimsIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Security.Claims;

/// <summary>
/// Service interface for managing role identity claims indexing.
/// </summary>
/// <remarks>
/// This service provides functionality to maintain and query indexes of role claims,
/// enabling efficient lookups of roles by their claim values.
/// </remarks>
public interface IRoleClaimsIndexService
{
    /// <summary>
    /// Adds a claim value to the role's index.
    /// </summary>
    /// <param name="claimType">The type of the claim being indexed.</param>
    /// <param name="claimValue">The value of the claim being indexed.</param>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string claimType, string claimValue, string roleId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a claim value to the role's index.
    /// </summary>
    /// <param name="claim">The claim to search for.</param>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Claim claim, string roleId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds all role IDs associated with a specific claim.
    /// </summary>
    /// <param name="claim">The claim to search for.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A collection of role IDs that have the specified claim.</returns>
    /// <remarks>
    /// This method searches the claims index to find all roles who have been assigned the specified claim.
    /// The search is performed using both the claim type and value.
    /// </remarks>
    Task<IEnumerable<string>> FindRoleIdsAsync(Claim claim, CancellationToken cancellationToken);

    /// <summary>
    /// Finds all role IDs associated with a specific claim type and value.
    /// </summary>
    /// <param name="claimType">The type of claim to search for.</param>
    /// <param name="claimValue">The value of the claim to search for.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A collection of role IDs that have the specified claim type and value.</returns>
    /// <remarks>
    /// This method provides a way to search for roles by specifying the claim type and value separately,
    /// rather than passing a Claim object.
    /// </remarks>
    Task<IEnumerable<string>> FindRoleIdsAsync(string claimType, string claimValue, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a specific claim from a role's index.
    /// </summary>
    /// <param name="claim">The claim to remove.</param>
    /// <param name="roleId">The ID of the role from whom to remove the claim.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method removes the index entry for the specified claim associated with the given role.
    /// The actual claim on the role's identity is not modified by this operation.
    /// </remarks>
    Task RemoveAsync(Claim claim, string roleId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a specific claim type and value from a role's index.
    /// </summary>
    /// <param name="claimType">The type of claim to remove.</param>
    /// <param name="claimValue">The value of the claim to remove.</param>
    /// <param name="roleId">The ID of the role from whom to remove the claim.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method provides a way to remove an index entry by specifying the claim type and value separately,
    /// rather than passing a Claim object. The actual claim on the role's identity is not modified by this operation.
    /// </remarks>
    Task RemoveAsync(string claimType, string claimValue, string roleId, CancellationToken cancellationToken);
}