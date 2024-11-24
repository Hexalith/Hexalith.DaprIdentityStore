// <copyright file="UserActor{Users}.cs" company="ITANEO">
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
/// Represents a Dapr actor for managing user identity operations.
/// </summary>
/// <param name="host">The actor host that provides runtime context.</param>
/// <param name="collectionService">Service for managing the user collection.</param>
/// <param name="emailIndexService">Service for managing email-based user indexing.</param>
/// <param name="nameIndexService">Service for managing username-based user indexing.</param>
/// <param name="claimIndexService">Service for managing claim-based user indexing.</param>
/// <param name="tokenIndexService">Service for managing token-based user indexing.</param>
/// <param name="loginIndexService">Service for managing login-based user indexing.</param>
public partial class UserActor(
    ActorHost host,
    IUserCollectionService collectionService,
    IUserEmailIndexService emailIndexService,
    IUserNameIndexService nameIndexService,
    IUserClaimsIndexService claimIndexService,
    IUserTokenIndexService tokenIndexService,
    IUserLoginIndexService loginIndexService)
    : Actor(host), IUserActor
{
    private readonly IUserClaimsIndexService _claimIndexService = claimIndexService;

    /// <summary>
    /// Collection services for managing different aspects of user identity.
    /// </summary>
    private readonly IUserCollectionService _collectionService = collectionService;

    private readonly IUserEmailIndexService _emailCollectionService = emailIndexService;

    // Manages email-based indexing
    private readonly IUserLoginIndexService _loginIndexService = loginIndexService;

    // Manages the main user collection
    private readonly IUserNameIndexService _nameCollectionService = nameIndexService;

    private readonly IUserTokenIndexService _tokenIndexService = tokenIndexService;

    // Manages username-based indexing

    /// <summary>
    /// Cached state of the user actor to minimize state store access.
    /// State is lazily loaded and cached for performance.
    /// </summary>
    private UserActorState? _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserActor"/> class for testing purposes.
    /// </summary>
    /// <param name="host">The actor host that provides runtime context.</param>
    /// <param name="collectionService">Service for managing the user collection.</param>
    /// <param name="emailCollectionService">Service for managing email-based user indexing.</param>
    /// <param name="nameCollectionService">Service for managing username-based user indexing.</param>
    /// <param name="claimIndexService">Service for managing claim-based user indexing.</param>
    /// <param name="tokenIndexService">Service for managing token-based user indexing.</param>
    /// <param name="loginCollectionService">Service for managing login-based user indexing.</param>
    /// <param name="stateManager">Optional state manager for managing actor state.</param>
    internal UserActor(
        ActorHost host,
        IUserCollectionService collectionService,
        IUserEmailIndexService emailCollectionService,
        IUserNameIndexService nameCollectionService,
        IUserClaimsIndexService claimIndexService,
        IUserTokenIndexService tokenIndexService,
        IUserLoginIndexService loginCollectionService,
        IActorStateManager stateManager)
        : this(
              host,
              collectionService,
              emailCollectionService,
              nameCollectionService,
              claimIndexService,
              tokenIndexService,
              loginCollectionService) => StateManager = stateManager;

    /// <summary>
    /// Creates a new user identity and establishes all necessary indexes.
    /// This includes adding the user to the main collection and creating email/username indexes if provided.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if creation was successful, false if user already exists.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID doesn't match actor ID.</exception>
    public async Task<bool> CreateAsync(CustomUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

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

        await _collectionService.AddAsync(user.Id);

        // Create email index if email exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            await _emailCollectionService.AddAsync(user.NormalizedEmail, user.Id);
        }

        // Create username index if username exists
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            await _nameCollectionService.AddAsync(user.NormalizedUserName, user.Id);
        }

        // Save user state
        await StateManager.AddStateAsync(DaprIdentityStoreConstants.UserStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
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
                await _emailCollectionService.RemoveAsync(_state.User.NormalizedEmail);
            }

            // Remove username index
            if (!string.IsNullOrWhiteSpace(_state.User.NormalizedUserName))
            {
                await _nameCollectionService.RemoveAsync(_state.User.NormalizedUserName);
            }

            // Clear state
            _state = null;
            await StateManager.RemoveStateAsync(DaprIdentityStoreConstants.UserStateName, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
        }

        // Remove from indexes
        await _collectionService.RemoveAsync(id);
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
    public async Task<CustomUser?> FindAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state?.User;
    }

    /// <summary>
    /// Updates an existing user's information and maintains associated indexes.
    /// This includes updating email and username indexes if they have changed.
    /// </summary>
    /// <param name="user">The updated user information.</param>
    /// <exception cref="InvalidOperationException">Thrown when user doesn't exist.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(CustomUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Update Failed : User '{user.Id}' not found.");
        }

        // Update user state
        CustomUser oldUser = _state.User;
        _state.User = user;
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        // Update email index
        if (oldUser.NormalizedEmail != user.NormalizedEmail)
        {
            if (!string.IsNullOrWhiteSpace(oldUser.NormalizedEmail))
            {
                await _emailCollectionService.RemoveAsync(oldUser.NormalizedEmail);
            }

            if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
            {
                await _emailCollectionService.AddAsync(user.NormalizedEmail, user.Id);
            }
        }

        // Update username index
        if (oldUser.NormalizedUserName != user.NormalizedUserName)
        {
            if (!string.IsNullOrWhiteSpace(oldUser.NormalizedUserName))
            {
                await _nameCollectionService.RemoveAsync(oldUser.NormalizedUserName);
            }

            if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
            {
                await _nameCollectionService.AddAsync(user.NormalizedUserName, user.Id);
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
            ConditionalValue<UserActorState> result = await StateManager.TryGetStateAsync<UserActorState>(DaprIdentityStoreConstants.UserStateName, cancellationToken);
            if (result.HasValue)
            {
                _state = result.Value;
            }
        }

        return _state;
    }
}