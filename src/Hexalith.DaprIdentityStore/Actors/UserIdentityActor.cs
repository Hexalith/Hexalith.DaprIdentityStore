// <copyright file="UserIdentityActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// Handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
/// Initializes a new instance of the UserIdentityActor.
/// </remarks>
public class UserIdentityActor : Actor, IUserIdentityActor
{
    /// <summary>
    /// Cached state of the user actor to minimize state store access.
    /// </summary>
    private UserActorState? _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
    /// </summary>
    /// <param name="host">The actor host that provides runtime context.</param>
    /// <param name="stateManager">The state manager for managing actor state.</param>
    public UserIdentityActor(ActorHost host, IActorStateManager? stateManager = null)
        : base(host)
    {
        if (stateManager is not null)
        {
            StateManager = stateManager;
        }
    }

    /// <summary>
    /// Adds claims to the user identity.
    /// </summary>
    /// <param name="claims">The claims to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user state is not found.</exception>
    public async Task AddClaimsAsync(IEnumerable<Claim> claims)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add state Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        // Add claims to user state and remove duplicates
        IEnumerable<ApplicationUserClaim> newClaims = claims
            .Select(p => new ApplicationUserClaim { UserId = Id.ToUnescapeString(), ClaimType = p.Type, ClaimValue = p.Value });
        _state.Claims = _state.Claims.Union(newClaims).Distinct();
    }

    /// <summary>
    /// Creates a new user identity and associated indexes.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if creation was successful, false if user already exists.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID doesn't match actor ID.</exception>
    public async Task<bool> CreateAsync(UserIdentity user)
    {
        // Validate user ID matches actor ID
        if (user.Id != Id.ToUnescapeString())
        {
            throw new InvalidOperationException($"{Host.ActorTypeInfo.ActorTypeName} Id '{Id.ToUnescapeString()}' does not match user Id '{user.Id}'.");
        }

        // Return false if user already exists
        if (_state != null)
        {
            return false;
        }

        _state = new UserActorState { User = user };

        // Save user state
        await StateManager.AddStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        // Create indexes for user lookup
        IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        _ = await collection.AddAsync(user.Id);

        // Create email index if email exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(user.NormalizedEmail);
            await emailIndex.SetAsync(user.Id);
        }

        // Create username index if username exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(user.NormalizedUserName);
            await nameIndex.SetAsync(user.Id);
        }

        return true;
    }

    /// <summary>
    /// Deletes a user identity and removes associated indexes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DeleteAsync()
    {
        string id = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is not null)
        {
            // Remove email index
            if (!string.IsNullOrWhiteSpace(_state.User.NormalizedEmail))
            {
                IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(_state.User.NormalizedEmail);
                await emailIndex.RemoveAsync();
            }

            // Remove username index
            if (!string.IsNullOrWhiteSpace(_state.User.NormalizedUserName))
            {
                IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(_state.User.NormalizedUserName);
                await nameIndex.RemoveAsync();
            }

            // Clear state
            _state = null;
            await StateManager.RemoveStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
        }

        // Remove from indexes
        IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        await collection.RemoveAsync(id);
    }

    /// <summary>
    /// Checks if the user exists.
    /// </summary>
    /// <returns>True if user exists, false otherwise.</returns>
    public async Task<bool> ExistsAsync() => await GetStateAsync(CancellationToken.None) != null;

    /// <summary>
    /// Finds a user by their ID.
    /// </summary>
    /// <returns>The user identity if found, null otherwise.</returns>
    public async Task<UserIdentity?> FindAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state?.User;
    }

    /// <inheritdoc/>
    public Task<IList<Claim>> GetClaimsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task RemoveClaimsAsync(IEnumerable<Claim> claims)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add state Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        // Add claims to user state and remove duplicates
        IEnumerable<ApplicationUserClaim> newClaims = claims
            .Select(p => new ApplicationUserClaim { UserId = Id.ToUnescapeString(), ClaimType = p.Type, ClaimValue = p.Value });
        _state.Claims = _state.Claims.Union(newClaims).Distinct();
    }

    /// <inheritdoc/>
    public async Task ReplaceClaimAsync(Claim claim, Claim newClaim)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add state Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        // Add claims to user state and remove duplicates
        IEnumerable<ApplicationUserClaim> newClaims = _state.Claims.Where(p => p.ClaimType != claim.Type);
        _state.Claims = _state.Claims.Union([new ApplicationUserClaim { UserId = Id.ToUnescapeString(), ClaimType = newClaim.ValueType, ClaimValue = newClaim.Value }]);
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Updates an existing user's information and associated indexes.
    /// </summary>
    /// <param name="user">The updated user information.</param>
    /// <exception cref="InvalidOperationException">Thrown when user doesn't exist.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UpdateAsync(UserIdentity user)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Update Failed : User '{user.Id}' not found.");
        }

        // Update user state
        _ = _state.User;
        _state.User = user;
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        // Update indexes
        IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        await collection.RemoveAsync(user.Id);

        // Update email index
        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(user.NormalizedEmail);
            await emailIndex.RemoveAsync();
        }

        // Update username index
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(user.NormalizedUserName);
            await nameIndex.RemoveAsync();
        }
    }

    /// <summary>
    /// Retrieves the actor's state from the state store if not already cached.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The actor's state if it exists, null otherwise.</returns>
    private async Task<UserActorState?> GetStateAsync(CancellationToken cancellationToken)
    {
        if (_state is null)
        {
            ConditionalValue<UserActorState> result = await StateManager.TryGetStateAsync<UserActorState>(DaprIdentityStoreConstants.UserIdentityStateName, cancellationToken);
            if (result.HasValue)
            {
                _state = result.Value;
            }
        }

        return _state;
    }
}