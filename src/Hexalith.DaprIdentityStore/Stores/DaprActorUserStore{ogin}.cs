// <copyright file="DaprActorUserStore{ogin}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user store that uses Dapr actors for user management.
/// </summary>
public partial class DaprActorUserStore
    : UserStoreBase<CustomUser, string, CustomUserClaim, CustomUserLogin, CustomUserToken>
{
    /// <inheritdoc/>
    public override async Task AddLoginAsync(CustomUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(login);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(login.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(login.ProviderKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.AddLoginAsync(login);
    }

    /// <inheritdoc/>
    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(CustomUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        return (await actor.GetLoginsAsync()).ToList();
    }

    /// <inheritdoc/>
    public override async Task RemoveLoginAsync(CustomUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.RemoveLoginAsync(loginProvider, providerKey);
    }

    /// <inheritdoc/>
    protected override async Task<CustomUserLogin?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        return await actor.FindLoginAsync(loginProvider, providerKey);
    }

    /// <inheritdoc/>
    protected override async Task<CustomUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        string? userId = await _loginCollectionService.FindUserIdAsync(loginProvider, providerKey);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        return await actor.FindLoginAsync(loginProvider, providerKey);
    }
}