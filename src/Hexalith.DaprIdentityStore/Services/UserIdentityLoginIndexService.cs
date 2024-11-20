// <copyright file="UserIdentityLoginIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System;
using System.Threading.Tasks;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity logins in a collection.
/// This service handles the mapping between user IDs and their external login providers using Dapr actors.
/// It provides functionality to add, find, and remove login provider mappings.
/// </summary>
public class UserIdentityLoginIndexService : IUserIdentityLoginIndexService
{
    // Factory function to create key-value actors for login indexing
    private readonly Func<string, string, IKeyValueActor> _keyValueActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityLoginIndexService"/> class.
    /// This constructor is used in production with actual Dapr actor implementation.
    /// </summary>
    /// <param name="actorHost">The Dapr actor host providing actor management capabilities.</param>
    public UserIdentityLoginIndexService(ActorHost actorHost) =>

        // Initialize the factory with the login index proxy creator
        _keyValueActor = actorHost.ProxyFactory.CreateUserLoginIndexProxy;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityLoginIndexService"/> class.
    /// This constructor is primarily used for testing, allowing injection of mock actors.
    /// </summary>
    /// <param name="keyValueActor">Factory function to create key-value actors.</param>
    internal UserIdentityLoginIndexService(Func<string, string, IKeyValueActor> keyValueActor) =>
        _keyValueActor = keyValueActor;

    /// <inheritdoc/>
    public async Task AddUserLoginAsync(string id, string loginProvider, string providerKey) =>
        await _keyValueActor(loginProvider, providerKey).SetAsync(id);

    /// <inheritdoc/>
    public async Task<string?> FindUserByLoginAsync(string loginProvider, string providerKey) =>
        await _keyValueActor(loginProvider, providerKey).GetAsync();

    /// <inheritdoc/>
    public async Task RemoveUserLoginAsync(string id, string loginProvider, string providerKey) =>
        await _keyValueActor(loginProvider, providerKey).RemoveAsync();
}