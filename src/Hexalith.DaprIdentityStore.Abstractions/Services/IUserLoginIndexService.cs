// <copyright file="IUserLoginIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

/// <summary>
/// Defines the interface for managing user identity logins in a collection.
/// This service handles the mapping between user IDs and their external login providers.
/// </summary>
public interface IUserLoginIndexService
{
    /// <summary>
    /// Associates a user ID with an external login provider in the actor state store.
    /// </summary>
    /// <param name="loginProvider">The name of the external login provider.</param>
    /// <param name="providerKey">The provider-specific key for the user.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(string loginProvider, string providerKey, string userId);

    /// <summary>
    /// Retrieves a user ID associated with the given external login provider information.
    /// </summary>
    /// <param name="loginProvider">The name of the external login provider.</param>
    /// <param name="providerKey">The provider-specific key for the user.</param>
    /// <returns>The associated user ID if found; otherwise, null.</returns>
    Task<string?> FindUserIdAsync(string loginProvider, string providerKey);

    /// <summary>
    /// Removes the association between a user ID and an external login provider.
    /// </summary>
    /// <param name="loginProvider">The name of the external login provider.</param>
    /// <param name="providerKey">The provider-specific key for the user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RemoveAsync(string loginProvider, string providerKey);
}