// <copyright file="RoleIdentityActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Represents a Dapr actor for managing role identity operations.
/// </summary>
/// <param name="host">The actor host that provides runtime context.</param>
/// <param name="collectionService">Service for managing the role collection.</param>
/// <param name="nameIndexService">Service for managing name-based role indexing.</param>
/// <param name="claimsIndexService">Service for managing claim-based role indexing.</param>
public partial class RoleActor(
    ActorHost host,
    IRoleCollectionService collectionService,
    IRoleNameIndexService nameIndexService,
    IRoleClaimsIndexService claimsIndexService)
    : Actor(host), IRoleActor
{
    private readonly IRoleClaimsIndexService _claimIndexService = claimsIndexService;

    /// <summary>
    /// Collection services for managing different aspects of role identity.
    /// </summary>
    private readonly IRoleCollectionService _collectionService = collectionService;

    // Manages the main role collection
    private readonly IRoleNameIndexService _nameCollectionService = nameIndexService;

    // Manages name-based indexing

    /// <summary>
    /// Cached state of the role actor to minimize state store access.
    /// State is lazily loaded and cached for performance.
    /// </summary>
    private RoleActorState? _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleActor"/> class for testing purposes.
    /// </summary>
    /// <param name="host">The actor host that provides runtime context.</param>
    /// <param name="collectionService">Service for managing the role collection.</param>
    /// <param name="nameCollectionService">Service for managing name-based role indexing.</param>
    /// <param name="claimIndexService">Service for managing claim-based role indexing.</param>
    /// <param name="stateManager">Optional state manager for managing actor state.</param>
    internal RoleActor(
        ActorHost host,
        IRoleCollectionService collectionService,
        IRoleNameIndexService nameCollectionService,
        IRoleClaimsIndexService claimIndexService,
        IActorStateManager stateManager)
        : this(
              host,
              collectionService,
              nameCollectionService,
              claimIndexService) => StateManager = stateManager;

    /// <summary>
    /// Creates a new role identity and establishes all necessary indexes.
    /// This includes adding the role to the main collection and creating email/name indexes if provided.
    /// </summary>
    /// <param name="role">The role identity to create.</param>
    /// <returns>True if creation was successful, false if role already exists.</returns>
    /// <exception cref="InvalidOperationException">Thrown when role ID doesn't match actor ID.</exception>
    public async Task<bool> CreateAsync(CustomRole role)
    {
        ArgumentNullException.ThrowIfNull(role);

        // Validate role ID matches actor ID
        if (role.Id != Id.ToUnescapeString())
        {
            throw new InvalidOperationException($"{Host.ActorTypeInfo.ActorTypeName} Id '{Id.ToUnescapeString()}' does not match role Id '{role.Id}'.");
        }

        // Return false if role already exists
        if (_state != null)
        {
            return false;
        }

        _state = new RoleActorState { Role = role };

        // Save role state
        await StateManager.AddStateAsync(DaprIdentityStoreConstants.RoleStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        await _collectionService.AddAsync(role.Id);

        // Create name index if name exists
        if (!string.IsNullOrWhiteSpace(role.NormalizedName))
        {
            await _nameCollectionService.AddAsync(role.NormalizedName, role.Id);
        }

        return true;
    }

    /// <summary>
    /// Deletes a role identity and removes all associated indexes.
    /// This includes removing the role from email and name indexes if they exist.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync()
    {
        string id = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is not null)
        {
            // Remove name index
            if (!string.IsNullOrWhiteSpace(_state.Role.NormalizedName))
            {
                await _nameCollectionService.RemoveAsync(_state.Role.NormalizedName);
            }

            // Clear state
            _state = null;
            await StateManager.RemoveStateAsync(DaprIdentityStoreConstants.RoleStateName, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
        }

        // Remove from indexes
        await _collectionService.RemoveAsync(id);
    }

    /// <summary>
    /// Checks if the role exists in the state store.
    /// </summary>
    /// <returns>True if role exists, false otherwise.</returns>
    public async Task<bool> ExistsAsync() => await GetStateAsync(CancellationToken.None) != null;

    /// <summary>
    /// Retrieves a role's identity information.
    /// </summary>
    /// <returns>The role identity if found, null otherwise.</returns>
    public async Task<CustomRole?> FindAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state?.Role;
    }

    /// <summary>
    /// Updates an existing role's information and maintains associated indexes.
    /// This includes updating email and name indexes if they have changed.
    /// </summary>
    /// <param name="role">The updated role information.</param>
    /// <exception cref="InvalidOperationException">Thrown when role doesn't exist.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(CustomRole role)
    {
        ArgumentNullException.ThrowIfNull(role);
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Update Failed : Role '{role.Id}' not found.");
        }

        // Update role state
        CustomRole oldRole = _state.Role;
        _state.Role = role;
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.RoleStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        // Update name index
        if (oldRole.NormalizedName != role.NormalizedName)
        {
            if (!string.IsNullOrWhiteSpace(oldRole.NormalizedName))
            {
                await _nameCollectionService.RemoveAsync(oldRole.NormalizedName);
            }

            if (!string.IsNullOrWhiteSpace(role.NormalizedName))
            {
                await _nameCollectionService.AddAsync(role.NormalizedName, role.Id);
            }
        }
    }

    /// <summary>
    /// Retrieves the actor's state from the state store if not already cached.
    /// Uses lazy loading pattern to minimize state store access.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The actor's state if it exists, null otherwise.</returns>
    private async Task<RoleActorState?> GetStateAsync(CancellationToken cancellationToken)
    {
        if (_state is null)
        {
            ConditionalValue<RoleActorState> result = await StateManager.TryGetStateAsync<RoleActorState>(DaprIdentityStoreConstants.RoleStateName, cancellationToken);
            if (result.HasValue)
            {
                _state = result.Value;
            }
        }

        return _state;
    }
}