// <copyright file="UserIdentityEmailIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Threading.Tasks;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity emails in a collection.
/// This service handles the mapping between user IDs and their email addresses using Dapr actors.
/// It provides functionality to add, find, and remove email-to-userId mappings.
/// </summary>
public class UserIdentityEmailIndexService : IUserIdentityEmailIndexService
{
    // Factory function to create key-value actors for email indexing
    private readonly Func<string, IKeyValueActor> _keyValueActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityEmailIndexService"/> class.
    /// This constructor is used in production with actual Dapr actor implementation.
    /// </summary>
    /// <param name="actorHost">The Dapr actor host providing actor management capabilities.</param>
    public UserIdentityEmailIndexService(ActorHost actorHost) =>

        // Initialize the factory with the email index proxy creator
        _keyValueActor = actorHost.ProxyFactory.CreateUserEmailIndexProxy;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityEmailIndexService"/> class.
    /// This constructor is primarily used for testing, allowing injection of mock actors.
    /// </summary>
    /// <param name="keyValueActor">Factory function to create key-value actors.</param>
    internal UserIdentityEmailIndexService(Func<string, IKeyValueActor> keyValueActor) => _keyValueActor = keyValueActor;

    /// <summary>
    /// Associates a user ID with an email address in the actor state store.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="email">The email address to associate with the user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddUserEmailAsync(string id, string email) => await _keyValueActor(email).SetAsync(id);

    /// <summary>
    /// Retrieves a user ID associated with the given email address.
    /// </summary>
    /// <param name="email">The email address to look up.</param>
    /// <returns>The associated user ID if found; otherwise, null.</returns>
    public async Task<string?> FindUserByEmailAsync(string email) => await _keyValueActor(email).GetAsync();

    /// <summary>
    /// Removes the association between a user ID and an email address.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="email">The email address to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveUserEmailAsync(string id, string email) => await _keyValueActor(email).RemoveAsync();
}