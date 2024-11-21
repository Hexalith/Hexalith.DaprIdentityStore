// <copyright file="IRoleCollectionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Provides operations for managing role identity collections.
/// </summary>
public interface IRoleCollectionService
{
    /// <summary>
    /// Adds a new role to the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier for the role.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string id);

    /// <summary>
    /// Retrieves all role identifiers from the collection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns an enumerable of role identifiers.</returns>
    Task<IEnumerable<string>> AllAsync();

    /// <summary>
    /// Removes a role and all their associated data from the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the role to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string id);
}