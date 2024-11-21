// <copyright file="IUserActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Dapr.Actors;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a Dapr actor interface for managing user identity operations.
/// </summary>
public interface IUserActor : IActor
{
    /// <summary>
    /// Adds a collection of claims to the user identity asynchronously.
    /// </summary>
    /// <param name="claims">The collection of claims to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddClaimsAsync(IEnumerable<Claim> claims);

    /// <summary>
    /// Adds a login to the user identity asynchronously.
    /// </summary>
    /// <param name="login">The login information to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLoginAsync(UserLoginInfo login);

    /// <summary>
    /// Adds a token to the user identity asynchronously.
    /// </summary>
    /// <param name="token">The token to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddTokenAsync(CustomUserToken token);

    /// <summary>
    /// Creates a new user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if the creation was successful; otherwise, false.</returns>
    Task<bool> CreateAsync(CustomUser user);

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
    Task<CustomUser?> FindAsync();

    /// <summary>
    /// Finds a login for a user identity asynchronously.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="providerKey">The provider key.</param>
    /// <returns>The user login information if found; otherwise, null.</returns>
    Task<CustomUserLogin?> FindLoginAsync(string loginProvider, string providerKey);

    /// <summary>
    /// Retrieves all claims associated with the user identity asynchronously.
    /// </summary>
    /// <returns>A collection of claims associated with the user.</returns>
    Task<IEnumerable<Claim>> GetClaimsAsync();

    /// <summary>
    /// Retrieves all logins associated with the user identity asynchronously.
    /// </summary>
    /// <returns>A collection of user login information.</returns>
    Task<IEnumerable<UserLoginInfo>> GetLoginsAsync();

    /// <summary>
    /// Retrieves a token associated with the user identity asynchronously.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    /// <returns>The user token if found; otherwise, null.</returns>
    Task<CustomUserToken?> GetTokenAsync(string loginProvider, string name);

    /// <summary>
    /// Removes a collection of claims from the user identity asynchronously.
    /// </summary>
    /// <param name="claims">The collection of claims to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveClaimsAsync(IEnumerable<Claim> claims);

    /// <summary>
    /// Removes a login from the user identity asynchronously.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="providerKey">The provider key.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveLoginAsync(string loginProvider, string providerKey);

    /// <summary>
    /// Removes a token from the user identity asynchronously.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveTokenAsync(string loginProvider, string name);

    /// <summary>
    /// Replaces an existing claim with a new claim asynchronously.
    /// </summary>
    /// <param name="claim">The existing claim to replace.</param>
    /// <param name="newClaim">The new claim that will replace the existing claim.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReplaceClaimAsync(Claim claim, Claim newClaim);

    /// <summary>
    /// Updates an existing user identity asynchronously.
    /// </summary>
    /// <param name="user">The user identity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(CustomUser user);
}