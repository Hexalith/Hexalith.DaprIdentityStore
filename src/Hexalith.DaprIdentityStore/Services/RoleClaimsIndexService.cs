// <copyright file="RoleClaimsIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Security.Claims;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing role identity claims indexing using Dapr actors.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleClaimsIndexService"/> class.
/// </remarks>
/// <param name="actorProxyFactory">The actor proxy factory instance.</param>
public class RoleClaimsIndexService(
    IActorProxyFactory actorProxyFactory) : IRoleClaimsIndexService
{
    private readonly Func<string, string, IKeyHashActor> _keyValueActor = actorProxyFactory.CreateClaimRolesIndexProxy;

    /// <inheritdoc/>
    public async Task AddAsync(string claimType, string claimValue, string roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);
        ArgumentNullException.ThrowIfNull(roleId);

        _ = await _keyValueActor(claimType, claimValue).AddAsync(roleId);
    }

    /// <inheritdoc/>
    public Task AddAsync(Claim claim, string roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(roleId);

        return AddAsync(claim.Type, claim.Value, roleId, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<string>> FindRoleIdsAsync(Claim claim, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);

        return FindRoleIdsAsync(claim.Type, claim.Value, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> FindRoleIdsAsync(string claimType, string claimValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);

        return await _keyValueActor(claimType, claimValue).AllAsync();
    }

    /// <inheritdoc/>
    public Task RemoveAsync(Claim claim, string roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(roleId);

        return RemoveAsync(claim.Type, claim.Value, roleId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string claimType, string claimValue, string roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);
        ArgumentNullException.ThrowIfNull(roleId);

        await _keyValueActor(claimType, claimValue).RemoveAsync(roleId);
    }
}