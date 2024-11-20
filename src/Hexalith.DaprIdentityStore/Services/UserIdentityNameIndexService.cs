// <copyright file="UserIdentityNameIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity names in a collection.
/// This service handles the mapping between user IDs and their usernames using Dapr actors.
/// It provides functionality to add, find, and remove username-to-userId mappings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityNameIndexService"/> class.
/// This constructor is used in production with actual Dapr actor implementation.
/// </remarks>
/// <param name="factory">The Dapr actor host providing actor management capabilities.</param>
public class UserIdentityNameIndexService(IActorProxyFactory factory) : IUserIdentityNameIndexService
{
    // Factory function to create key-value actors for username indexing
    private readonly Func<string, IKeyValueActor> _keyValueActor = factory.CreateUserNameIndexProxy;

    /// <summary>
    /// Associates a user ID with a username in the actor state store.
    /// </summary>
    /// <param name="name">The username to associate with the user.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(string name, string userId)
        => await _keyValueActor(name).SetAsync(userId);

    /// <summary>
    /// Retrieves a user ID associated with the given username.
    /// </summary>
    /// <param name="name">The username to look up.</param>
    /// <returns>The associated user ID if found; otherwise, null.</returns>
    public async Task<string?> FindUserIdAsync(string name)
        => await _keyValueActor(name).GetAsync();

    /// <summary>
    /// Removes the association between a user ID and a username.
    /// </summary>
    /// <param name="name">The username to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveAsync(string name)
        => await _keyValueActor(name).RemoveAsync();
}