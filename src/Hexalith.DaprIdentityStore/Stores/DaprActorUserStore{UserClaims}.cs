// <copyright file="DaprActorUserStore{UserClaims}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
using System.Security.Claims;
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
    public override async Task AddClaimsAsync(UserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.AddClaimsAsync(claims);
    }

    /// <inheritdoc/>
    public override async Task<IList<Claim>> GetClaimsAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        return (await actor.GetClaimsAsync()).ToList();
    }

    /// <inheritdoc/>
    public override async Task<IList<UserIdentity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IEnumerable<string> allUsers = await _userIdentityCollection.AllAsync();
        List<Task<UserIdentity?>> userTasks = [];
        foreach (string userId in allUsers)
        {
            userTasks.Add(GetUserIfHasClaimAsync(claim, userId));
        }

        return (await Task.WhenAll(userTasks))
            .Where(p => p != null)
            .Select(p => p!)
            .ToList();
    }

    /// <inheritdoc/>
    public override async Task RemoveClaimsAsync(UserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.RemoveClaimsAsync(claims);
    }

    /// <inheritdoc/>
    public override async Task ReplaceClaimAsync(UserIdentity user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.ReplaceClaimAsync(claim, newClaim);
    }

    private static async Task<UserIdentity?> GetUserIfHasClaimAsync(Claim claim, string userId)
    {
        IUserIdentityActor collection = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        if ((await collection.GetClaimsAsync()).Any(p => p.Type == claim.Type && p.Value == claim.Value))
        {
            IUserIdentityActor userActor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
            return await userActor.FindAsync();
        }

        return null;
    }
}