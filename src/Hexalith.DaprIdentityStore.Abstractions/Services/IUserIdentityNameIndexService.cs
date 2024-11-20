// <copyright file="IUserIdentityNameIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Provides operations for managing user identity collections.
/// </summary>
public interface IUserIdentityNameIndexService
{
    /// <summary>
    /// Associates a username with a user in the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The username to associate with the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddUserNameAsync(string id, string name);

    /// <summary>
    /// Finds a user identifier by their associated username.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>A task representing the asynchronous operation that returns the user identifier if found, otherwise null.</returns>
    Task<string?> FindUserByNameAsync(string name);

    /// <summary>
    /// Removes a username association from a user in the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The username to remove from the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveUserNameAsync(string id, string name);
}