// <copyright file="DaprActorUserStore{UserToken}.cs" company="ITANEO">
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
    : UserStoreBase<UserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    /// <inheritdoc/>
    protected override async Task AddUserTokenAsync(ApplicationUserToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.UserId);
        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(token.UserId);
        await actor.AddTokenAsync(token);
    }

    /// <inheritdoc/>
    protected override async Task<ApplicationUserToken?> FindTokenAsync(UserIdentity user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        return await actor.GetTokenAsync(loginProvider, name);
    }

    /// <inheritdoc/>
    protected override async Task RemoveUserTokenAsync(ApplicationUserToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.LoginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token.UserId);
        ThrowIfDisposed();
        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(token.UserId);
        await actor.RemoveTokenAsync(token.LoginProvider, token.Name);
    }
}