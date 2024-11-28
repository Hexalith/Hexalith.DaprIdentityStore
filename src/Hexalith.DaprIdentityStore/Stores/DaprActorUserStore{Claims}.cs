// <copyright file="DaprActorUserStore{Claims}.cs" company="ITANEO">
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
    : UserStoreBase<CustomUser, string, CustomUserClaim, CustomUserLogin, CustomUserToken>
{
    /// <inheritdoc/>
    public override async Task AddClaimsAsync(CustomUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        await actor.AddClaimsAsync(claims.Select(p => new CustomUserClaim { ClaimType = p.Type, ClaimValue = p.Value, UserId = user.Id }));
    }

    /// <inheritdoc/>
    public override async Task<IList<Claim>> GetClaimsAsync(CustomUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        return (IList<Claim>)(await actor.GetClaimsAsync()).ToList();
    }

    /// <inheritdoc/>
    public override async Task<IList<CustomUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IEnumerable<string> allUsers = await _userCollection.AllAsync();
        List<Task<CustomUser?>> userTasks = [];
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
    public override async Task RemoveClaimsAsync(CustomUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        await actor.RemoveClaimsAsync(claims.Select(p => new CustomUserClaim { ClaimType = p.Type, ClaimValue = p.Value }));
    }

    /// <inheritdoc/>
    public override async Task ReplaceClaimAsync(CustomUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        await actor.ReplaceClaimAsync(
            new CustomUserClaim { ClaimType = claim.Type, ClaimValue = claim.Value },
            new CustomUserClaim { ClaimType = newClaim.Type, ClaimValue = newClaim.Value });
    }

    private static async Task<CustomUser?> GetUserIfHasClaimAsync(Claim claim, string userId)
    {
        IUserActor collection = ActorProxy.DefaultProxyFactory.CreateUserActor(userId);
        if ((await collection.GetClaimsAsync()).Any(p => p.ClaimType == claim.Type && p.ClaimValue == claim.Value))
        {
            IUserActor userActor = ActorProxy.DefaultProxyFactory.CreateUserActor(userId);
            return await userActor.FindAsync();
        }

        return null;
    }
}