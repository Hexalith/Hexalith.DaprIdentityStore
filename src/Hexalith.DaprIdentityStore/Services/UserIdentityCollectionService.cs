// <copyright file="UserIdentityCollectionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing the collection of user identities.
/// This service handles basic user identity operations like adding, removing, and listing users.
/// </summary>
public class UserIdentityCollectionService : IUserIdentityCollectionService
{
    private readonly ActorHost _actorHost;
    private IKeyHashActor? _keyHashActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityCollectionService"/> class.
    /// </summary>
    /// <param name="actorHost">The actor host for creating actor proxies.</param>
    public UserIdentityCollectionService(ActorHost actorHost) => _actorHost = actorHost;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityCollectionService"/> class.
    /// This constructor is primarily used for testing purposes.
    /// </summary>
    /// <param name="keyHashActor">The key hash actor implementation.</param>
    internal UserIdentityCollectionService(IKeyHashActor keyHashActor)
    {
        // This constructor is primarily used for testing purposes.
        _keyHashActor = keyHashActor;
        _actorHost = null!;
    }

    private IKeyHashActor KeyHashActor => _keyHashActor ??= _actorHost.ProxyFactory.CreateAllUsersProxy();

    /// <inheritdoc/>
    public async Task AddUserAsync(string id) => await KeyHashActor.AddAsync(id);

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> AllAsync() => await KeyHashActor.AllAsync();

    /// <inheritdoc/>
    public async Task RemoveUserAsync(string id) => await KeyHashActor.RemoveAsync(id);
}