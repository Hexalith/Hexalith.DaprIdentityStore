// <copyright file="IUserIdentityEmailIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Provides operations for managing user identity collections.
/// </summary>
public interface IUserIdentityEmailIndexService
{
    /// <summary>
    /// Associates an email address with a user in the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="email">The email address to associate with the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddUserEmailAsync(string id, string email);

    /// <summary>
    /// Finds a user identifier by their associated email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>A task representing the asynchronous operation that returns the user identifier if found, otherwise null.</returns>
    Task<string?> FindUserByEmailAsync(string email);

    /// <summary>
    /// Removes an email association from a user in the identity collection.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="email">The email address to remove from the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveUserEmailAsync(string id, string email);
}