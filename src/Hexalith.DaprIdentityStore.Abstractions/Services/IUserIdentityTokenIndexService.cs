// <copyright file="IUserIdentityTokenIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Interface for user identity token index service.
/// </summary>
public interface IUserIdentityTokenIndexService
{
    /// <summary>
    /// Adds a user token to the index.
    /// </summary>
    /// <param name="token">The application user token.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(ApplicationUserToken token, string userId);

    /// <summary>
    /// Adds a user token to the index.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(string loginProvider, string name, string userId);

    /// <summary>
    /// Finds the user identifier associated with the specified token.
    /// </summary>
    /// <param name="token">The application user token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user identifier if found; otherwise, null.</returns>
    Task<string?> FindUserIdAsync(ApplicationUserToken token);

    /// <summary>
    /// Finds the user identifier associated with the specified token.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user identifier if found; otherwise, null.</returns>
    Task<string?> FindUserIdAsync(string loginProvider, string name);

    /// <summary>
    /// Removes the specified token from the index.
    /// </summary>
    /// <param name="token">The application user token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(ApplicationUserToken token);

    /// <summary>
    /// Removes the specified token from the index.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string loginProvider, string name);
}