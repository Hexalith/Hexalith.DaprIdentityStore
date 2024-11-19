// <copyright file="IUserIdentityActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Represents a Dapr actor interface for managing user identity operations.
/// </summary>
public interface IUserIdentityActor : IActor
{
    /// <summary>
    /// Creates a new user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if the creation was successful; otherwise, false.</returns>
    Task<bool> CreateAsync(UserIdentity user);

    /// <summary>
    /// Deletes a user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(UserIdentity user);

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

    /// <summary>
    /// Finds a user identity by email address asynchronously.
    /// </summary>
    /// <returns>The user identity if found; otherwise, null.</returns>
    Task<UserIdentity?> FindByEmailAsync();

    /// <summary>
    /// Finds a user identity by username asynchronously.
    /// </summary>
    /// <returns>The user identity if found; otherwise, null.</returns>
    Task<UserIdentity?> FindByNameAsync();

    /// <summary>
    /// Updates an existing user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(UserIdentity user);
}