// <copyright file="DaprActorUserStore{Token}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

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
    protected override async Task AddUserTokenAsync(CustomUserToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.UserId);
        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(token.UserId);
        await actor.AddTokenAsync(token);
    }

    /// <inheritdoc/>
    protected override async Task<CustomUserToken?> FindTokenAsync(CustomUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        return await actor.GetTokenAsync(loginProvider, name);
    }

    /// <inheritdoc/>
    protected override async Task RemoveUserTokenAsync(CustomUserToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.UserId);
        ThrowIfDisposed();
        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(token.UserId);
        await actor.RemoveTokenAsync(token.LoginProvider, token.Name);
    }
}