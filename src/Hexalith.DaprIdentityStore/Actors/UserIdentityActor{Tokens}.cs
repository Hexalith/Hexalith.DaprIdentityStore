// <copyright file="UserIdentityActor{Tokens}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
public partial class UserIdentityActor
{
    /// <summary>
    /// Adds an authentication token for a specific login provider.
    /// Used for storing refresh tokens or other provider-specific tokens.
    /// </summary>
    /// <param name="token">Token information to store.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddTokenAsync(CustomUserToken token)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add token failed : User '{userId}' not found.");
        }

        _state.Tokens = _state
            .Tokens
            .Where(p => p.Name != token.Name || p.LoginProvider != token.LoginProvider)
            .Union([token]);
        await _tokenIndexService.AddAsync(token, userId);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Retrieves a specific authentication token for a login provider.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="name">Name of the token.</param>
    /// <returns>Token information if found, null otherwise.</returns>
    public async Task<CustomUserToken?> GetTokenAsync(string loginProvider, string name)
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state is null
            ? throw new InvalidOperationException($"Get token failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Tokens.FirstOrDefault(p => p.Name == name && p.LoginProvider == loginProvider);
    }

    /// <summary>
    /// Removes an authentication token for a specific login provider.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="name">Name of the token to remove.</param>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveTokenAsync(string loginProvider, string name)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove token failed : User '{userId}' not found.");
        }

        _state.Tokens = _state.Tokens.Where(p => p.Name != name || p.LoginProvider != loginProvider);

        await _tokenIndexService.RemoveAsync(loginProvider, name);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }
}