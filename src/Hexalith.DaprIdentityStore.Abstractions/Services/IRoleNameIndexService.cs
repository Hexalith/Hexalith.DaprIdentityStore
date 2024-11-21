// <copyright file="IRoleNameIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Provides operations for managing role identity collections.
/// </summary>
public interface IRoleNameIndexService
{
    /// <summary>
    /// Associates a name with a role in the identity collection.
    /// </summary>
    /// <param name="name">The name to associate with the role.</param>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string name, string roleId);

    /// <summary>
    /// Finds a role identifier by their associated name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>A task representing the asynchronous operation that returns the role identifier if found, otherwise null.</returns>
    Task<string?> FindRoleIdAsync(string name);

    /// <summary>
    /// Removes a name association from a role in the identity collection.
    /// </summary>
    /// <param name="name">The name to remove from the role.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string name);
}