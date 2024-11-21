// <copyright file="UserClaimsIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Security.Claims;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity claims indexing using Dapr actors.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserClaimsIndexService"/> class.
/// </remarks>
/// <param name="actorProxyFactory">The actor proxy factory instance.</param>
public class UserClaimsIndexService(
    IActorProxyFactory actorProxyFactory) : IUserClaimsIndexService
{
    private readonly Func<string, string, IKeyHashActor> _keyValueActor = actorProxyFactory.CreateClaimUsersIndexProxy;

    /// <inheritdoc/>
    public async Task AddAsync(string claimType, string claimValue, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);
        ArgumentNullException.ThrowIfNull(userId);

        _ = await _keyValueActor(claimType, claimValue).AddAsync(userId);
    }

    /// <inheritdoc/>
    public Task AddAsync(Claim claim, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(userId);

        return AddAsync(claim.Type, claim.Value, userId, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<string>> FindUserIdsAsync(Claim claim, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);

        return FindUserIdsAsync(claim.Type, claim.Value, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> FindUserIdsAsync(string claimType, string claimValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);

        return await _keyValueActor(claimType, claimValue).AllAsync();
    }

    /// <inheritdoc/>
    public Task RemoveAsync(Claim claim, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(userId);

        return RemoveAsync(claim.Type, claim.Value, userId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string claimType, string claimValue, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);
        ArgumentNullException.ThrowIfNull(userId);

        await _keyValueActor(claimType, claimValue).RemoveAsync(userId);
    }
}