// <copyright file="UserCollectionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing the collection of user identities.
/// This service handles basic user identity operations like adding, removing, and listing users.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserCollectionService"/> class.
/// </remarks>
/// <param name="actorHost">The actor host for creating actor proxies.</param>
public class UserCollectionService(IActorProxyFactory actorHost) : IUserCollectionService
{
    private readonly IKeyHashActor _keyHashActor = actorHost.CreateAllUsersProxy();

    /// <inheritdoc/>
    public async Task AddAsync(string id) => await _keyHashActor.AddAsync(id);

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> AllAsync() => await _keyHashActor.AllAsync();

    /// <inheritdoc/>
    public async Task RemoveAsync(string id) => await _keyHashActor.RemoveAsync(id);
}