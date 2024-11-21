// <copyright file="IUserCollectionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Provides operations for managing user identity collections.
/// </summary>
public interface IUserCollectionService
{
    /// <summary>
    /// Adds a new user to the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier for the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string id);

    /// <summary>
    /// Retrieves all user identifiers from the collection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns an enumerable of user identifiers.</returns>
    Task<IEnumerable<string>> AllAsync();

    /// <summary>
    /// Removes a user and all their associated data from the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the user to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string id);
}