// <copyright file="DaprActorUserStore{UserLogin}.cs" company="ITANEO">
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
    : UserStoreBase<UserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    /// <inheritdoc/>
    public override async Task AddLoginAsync(UserIdentity user, UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(login);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(login.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(login.ProviderKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.AddLoginAsync(login);
    }

    /// <inheritdoc/>
    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        return (await actor.GetLoginsAsync()).ToList();
    }

    /// <inheritdoc/>
    public override async Task RemoveLoginAsync(UserIdentity user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.RemoveLoginAsync(user.Id, loginProvider, providerKey);
    }

    /// <inheritdoc/>
    protected override async Task<ApplicationUserLogin?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        return await actor.FindLoginAsync(userId, loginProvider, providerKey);
    }

    /// <inheritdoc/>
    protected override async Task<ApplicationUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        string? userId = await _loginCollectionService.FindUserByLoginAsync(loginProvider, providerKey);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        return await actor.FindLoginAsync(userId, loginProvider, providerKey);
    }
}