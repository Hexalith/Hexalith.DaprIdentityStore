// <copyright file="UserIdentityEmailIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity emails in a collection.
/// This service handles the mapping between user IDs and their email addresses using Dapr actors.
/// It provides functionality to add, find, and remove email-to-userId mappings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityEmailIndexService"/> class.
/// This constructor is used in production with actual Dapr actor implementation.
/// </remarks>
/// <param name="factory">The Dapr actor host providing actor management capabilities.</param>
public class UserIdentityEmailIndexService(IActorProxyFactory factory) : IUserEmailIndexService
{
    // Factory function to create key-value actors for email indexing
    private readonly Func<string, IKeyValueActor> _keyValueActor = factory.CreateUserEmailIndexProxy;

    /// <summary>
    /// Associates a user ID with an email address in the actor state store.
    /// </summary>
    /// <param name="email">The email address to associate with the user.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(string email, string userId) => await _keyValueActor(email).SetAsync(userId);

    /// <summary>
    /// Retrieves a user ID associated with the given email address.
    /// </summary>
    /// <param name="email">The email address to look up.</param>
    /// <returns>The associated user ID if found; otherwise, null.</returns>
    public async Task<string?> FindUserIdAsync(string email) => await _keyValueActor(email).GetAsync();

    /// <summary>
    /// Removes the association between a user ID and an email address.
    /// </summary>
    /// <param name="email">The email address to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveAsync(string email) => await _keyValueActor(email).RemoveAsync();
}