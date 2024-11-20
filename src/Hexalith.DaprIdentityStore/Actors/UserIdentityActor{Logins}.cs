// <copyright file="UserIdentityActor{Logins}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
public partial class UserIdentityActor
{
    /// <summary>
    /// Adds a third-party login provider to the user's account.
    /// </summary>
    /// <param name="login">Login information containing provider and key.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddLoginAsync(UserLoginInfo login)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add login failed : User '{userId}' not found.");
        }

        _state.Logins = _state
            .Logins
            .Where(p => p.LoginProvider != login.LoginProvider || p.ProviderKey != login.ProviderKey)
            .Union([new ApplicationUserLogin { LoginProvider = login.LoginProvider }]);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        await _loginIndexService.AddAsync(login.LoginProvider, login.ProviderKey, userId);
    }

    /// <summary>
    /// Finds a specific login provider entry for the user.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="providerKey">Unique key from the provider.</param>
    /// <returns>Login information if found, null otherwise.</returns>
    public async Task<ApplicationUserLogin?> FindLoginAsync(string loginProvider, string providerKey)
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state is null
            ? throw new InvalidOperationException($"Find login failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Logins.FirstOrDefault(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
    }

    /// <summary>
    /// Gets all external login providers associated with the user.
    /// </summary>
    /// <returns>Collection of login provider information.</returns>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    public async Task<IEnumerable<UserLoginInfo>> GetLoginsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return (IEnumerable<UserLoginInfo>)(_state is null
            ? throw new InvalidOperationException($"Get logins failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Logins);
    }

    /// <summary>
    /// Removes a specific login provider from the user's account.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="providerKey">Unique key from the provider.</param>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveLoginAsync(string loginProvider, string providerKey)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove login Failed : User '{userId}' not found.");
        }

        _state.Logins = _state.Logins.Where(p => p.ProviderKey != providerKey || p.LoginProvider != loginProvider);
        await _loginIndexService.RemoveAsync(loginProvider, providerKey);
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }
}