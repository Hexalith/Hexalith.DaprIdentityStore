// <copyright file="UserIdentityActor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Actors;

using System.Collections.Generic;
using System.Security.Claims;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
/// Initializes a new instance of the UserIdentityActor.
/// </remarks>
public class UserIdentityActor : Actor, IUserIdentityActor
{
    /// <summary>
    /// Collection services for managing different aspects of user identity.
    /// </summary>
    private readonly IUserIdentityCollectionService _collectionService;         // Manages the main user collection

    private readonly IUserIdentityEmailCollectionService _emailCollectionService; // Manages email-based indexing
    private readonly IUserIdentityNameCollectionService _nameCollectionService;   // Manages username-based indexing

    /// <summary>
    /// Cached state of the user actor to minimize state store access.
    /// State is lazily loaded and cached for performance.
    /// </summary>
    private UserActorState? _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
    /// </summary>
    /// <param name="host">The actor host that provides runtime context.</param>
    /// <param name="collectionService">Service for managing the user collection.</param>
    /// <param name="emailCollectionService">Service for managing email-based user indexing.</param>
    /// <param name="nameCollectionService">Service for managing username-based user indexing.</param>
    /// <param name="stateManager">Optional state manager for managing actor state (primarily used for testing).</param>
    public UserIdentityActor(
        ActorHost host,
        IUserIdentityCollectionService collectionService,
        IUserIdentityEmailCollectionService emailCollectionService,
        IUserIdentityNameCollectionService nameCollectionService,
        IActorStateManager? stateManager = null)
        : base(host)
    {
        if (stateManager is not null)
        {
            StateManager = stateManager;
        }

        _collectionService = collectionService;
        _emailCollectionService = emailCollectionService;
        _nameCollectionService = nameCollectionService;
    }

    /// <summary>
    /// Adds claims to the user identity.
    /// Claims are unique combinations of ClaimType and ClaimValue associated with a user.
    /// </summary>
    /// <param name="claims">Collection of claims to add to the user.</param>
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

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Creates a new user identity and establishes all necessary indexes.
    /// This includes adding the user to the main collection and creating email/username indexes if provided.
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

        await _collectionService.AddUserAsync(user.Id);

        // Create email index if email exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            await _emailCollectionService.AddUserEmailAsync(user.Id, user.NormalizedEmail);
        }

        // Create username index if username exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            await _nameCollectionService.AddUserNameAsync(user.Id, user.NormalizedUserName);
        }

        return true;
    }

    /// <summary>
    /// Deletes a user identity and removes all associated indexes.
    /// This includes removing the user from email and username indexes if they exist.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync()
    {
        string id = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is not null)
        {
            // Remove email index
            if (!string.IsNullOrWhiteSpace(_state.User.NormalizedEmail))
            {
                await _emailCollectionService.RemoveUserEmailAsync(id, _state.User.NormalizedEmail);
            }

            // Remove username index
            if (!string.IsNullOrWhiteSpace(_state.User.NormalizedUserName))
            {
                await _nameCollectionService.RemoveUserNameAsync(id, _state.User.NormalizedUserName);
            }

            // Clear state
            _state = null;
            await StateManager.RemoveStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
        }

        // Remove from indexes
        await _collectionService.RemoveUserAsync(id);
    }

    /// <summary>
    /// Checks if the user exists in the state store.
    /// </summary>
    /// <returns>True if user exists, false otherwise.</returns>
    public async Task<bool> ExistsAsync() => await GetStateAsync(CancellationToken.None) != null;

    /// <summary>
    /// Retrieves a user's identity information.
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
    /// Updates an existing user's information and maintains associated indexes.
    /// This includes updating email and username indexes if they have changed.
    /// </summary>
    /// <param name="user">The updated user information.</param>
    /// <exception cref="InvalidOperationException">Thrown when user doesn't exist.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(UserIdentity user)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Update Failed : User '{user.Id}' not found.");
        }

        // Update user state
        UserIdentity oldUser = _state.User;
        _state.User = user;
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        // Update email index
        if (oldUser.NormalizedEmail != user.NormalizedEmail)
        {
            if (!string.IsNullOrWhiteSpace(oldUser.NormalizedEmail))
            {
                await _emailCollectionService.RemoveUserEmailAsync(user.Id, oldUser.NormalizedEmail);
            }

            if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
            {
                await _emailCollectionService.AddUserEmailAsync(user.Id, user.NormalizedEmail);
            }
        }

        // Update username index
        if (oldUser.NormalizedUserName != user.NormalizedUserName)
        {
            if (!string.IsNullOrWhiteSpace(oldUser.NormalizedUserName))
            {
                await _nameCollectionService.RemoveUserNameAsync(user.Id, oldUser.NormalizedUserName);
            }

            if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
            {
                await _nameCollectionService.AddUserNameAsync(user.Id, user.NormalizedUserName);
            }
        }
    }

    /// <summary>
    /// Retrieves the actor's state from the state store if not already cached.
    /// Uses lazy loading pattern to minimize state store access.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
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