// <copyright file="RoleNameIndexService.cs" company="ITANEO">
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
/// Service for managing role identity names in a collection.
/// This service handles the mapping between role IDs and their rolenames using Dapr actors.
/// It provides functionality to add, find, and remove rolename-to-roleId mappings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleNameIndexService"/> class.
/// This constructor is used in production with actual Dapr actor implementation.
/// </remarks>
/// <param name="factory">The Dapr actor host providing actor management capabilities.</param>
public class RoleNameIndexService(IActorProxyFactory factory) : IRoleNameIndexService
{
    // Factory function to create key-value actors for rolename indexing
    private readonly Func<string, IKeyValueActor> _keyValueActor = factory.CreateRoleNameIndexProxy;

    /// <summary>
    /// Associates a role ID with a rolename in the actor state store.
    /// </summary>
    /// <param name="name">The rolename to associate with the role.</param>
    /// <param name="roleId">The role's unique identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(string name, string roleId)
        => await _keyValueActor(name).SetAsync(roleId);

    /// <summary>
    /// Retrieves a role ID associated with the given rolename.
    /// </summary>
    /// <param name="name">The rolename to look up.</param>
    /// <returns>The associated role ID if found; otherwise, null.</returns>
    public async Task<string?> FindRoleIdAsync(string name)
        => await _keyValueActor(name).GetAsync();

    /// <summary>
    /// Removes the association between a role ID and a rolename.
    /// </summary>
    /// <param name="name">The rolename to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveAsync(string name)
        => await _keyValueActor(name).RemoveAsync();
}