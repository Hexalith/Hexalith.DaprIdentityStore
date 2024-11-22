// <copyright file="DaprActorRoleStore{Claims}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System;
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
/// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
/// </summary>
public partial class DaprActorRoleStore : RoleStoreBase<CustomRole, string, CustomUserRole, CustomRoleClaim>
{
    /// <inheritdoc/>
    public override async Task AddClaimAsync(CustomRole role, Claim claim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.AddClaimsAsync([claim]);
    }

    /// <inheritdoc/>
    public override async Task<IList<Claim>> GetClaimsAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        return (await actor.GetClaimsAsync()).ToList();
    }

    /// <inheritdoc/>
    public override async Task RemoveClaimAsync(CustomRole role, Claim claim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.RemoveClaimsAsync([claim]);
    }
}