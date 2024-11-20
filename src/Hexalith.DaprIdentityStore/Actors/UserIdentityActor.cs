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

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
/// Initializes a new instance of the UserIdentityActor.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="UserIdentityActor"/> class.
/// </remarks>
/// <param name="host">The actor host that provides runtime context.</param>
/// <param name="collectionService">Service for managing the user collection.</param>
/// <param name="emailCollectionService">Service for managing email-based user indexing.</param>
/// <param name="nameCollectionService">Service for managing username-based user indexing.</param>
/// <param name="loginCollectionService"></param>
public class UserIdentityActor(
    ActorHost host,
    IUserIdentityCollectionService collectionService,
    IUserIdentityEmailIndexService emailCollectionService,
    IUserIdentityNameIndexService nameCollectionService,
    IUserIdentityLoginIndexService loginCollectionService)
    : Actor(host), IUserIdentityActor
{
    /// <summary>
    /// Collection services for managing different aspects of user identity.
    /// </summary>
    private readonly IUserIdentityCollectionService _collectionService = collectionService;         // Manages the main user collection

    private readonly IUserIdentityEmailIndexService _emailCollectionService = emailCollectionService; // Manages email-based indexing
    private readonly IUserIdentityNameIndexService _nameCollectionService = nameCollectionService;   // Manages username-based indexing

    /// <summary>
    /// Cached state of the user actor to minimize state store access.
    /// State is lazily loaded and cached for performance.
    /// </summary>
    private UserActorState? _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentityActor"/> class for testing purposes.
    /// </summary>
    /// <param name="host">The actor host that provides runtime context.</param>
    /// <param name="collectionService">Service for managing the user collection.</param>
    /// <param name="emailCollectionService">Service for managing email-based user indexing.</param>
    /// <param name="nameCollectionService">Service for managing username-based user indexing.</param>
    /// <param name="loginCollectionService">Service for managing login-based user indexing.</param>
    /// <param name="stateManager">Optional state manager for managing actor state.</param>
    internal UserIdentityActor(
        ActorHost host,
        IUserIdentityCollectionService collectionService,
        IUserIdentityEmailIndexService emailCollectionService,
        IUserIdentityNameIndexService nameCollectionService,
        IUserIdentityLoginIndexService loginCollectionService,
        IActorStateManager stateManager)
        : this(host, collectionService, emailCollectionService, nameCollectionService, loginCollectionService) => StateManager = stateManager;

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

    /// <inheritdoc/>
    public Task AddLoginAsync(UserLoginInfo login) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    /// <summary>
    /// Creates a new user identity and establishes all necessary indexes.
    /// This includes adding the user to the main collection and creating email/username indexes if provided.
    /// </summary>
    /// <param name="user">The user identity to create.</param>
    /// <returns>True if creation was successful, false if user already exists.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID doesn't match actor ID.</exception>
    public async Task<bool> CreateAsync(UserIdentity user)
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

        // Save user state
        await StateManager.AddStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        await _collectionService.AddUserAsync(user.Id);

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
    public Task<ApplicationUserLogin?> FindLoginAsync(string loginProvider, string providerKey) => throw new NotImplementedException();

    /// <summary>
    /// Gets all claims associated with the user.
    /// </summary>
    /// <returns>A list of claims associated with the user.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user state is not found.</exception>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state is null
            ? throw new InvalidOperationException($"Get claims Failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Claims
            .Select(c => new Claim(c.ClaimType ?? string.Empty, c.ClaimValue ?? string.Empty));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserLoginInfo>> GetLoginsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return (IEnumerable<UserLoginInfo>)(_state is null
            ? throw new InvalidOperationException($"Get logins Failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Logins);
    }

    /// <inheritdoc/>
    public Task<ApplicationUserToken?> GetTokenAsync(string loginProvider, string name) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task RemoveClaimsAsync(IEnumerable<Claim> claims)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove claims Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        // Add claims to user state and remove duplicates
        IEnumerable<ApplicationUserClaim> newClaims = claims
            .Select(p => new ApplicationUserClaim { UserId = Id.ToUnescapeString(), ClaimType = p.Type, ClaimValue = p.Value });
        _state.Claims = _state.Claims.Union(newClaims).Distinct();
    }

    /// <inheritdoc/>
    public async Task RemoveLoginAsync(string loginProvider, string providerKey)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove login Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        _state.Logins = _state.Logins.Where(p => p.ProviderKey != providerKey || p.LoginProvider != loginProvider);
        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task RemoveTokenAsync(string loginProvider, string name)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove login Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        _state.Tokens = _state.Tokens.Where(p => p.Name != name || p.LoginProvider != loginProvider);

        await StateManager.SetStateAsync(DaprIdentityStoreConstants.UserIdentityStateName, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task ReplaceClaimAsync(Claim claim, Claim newClaim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add state Failed : User '{Id.ToUnescapeString()}' not found.");
        }

        // Add claims to user state and remove duplicates
        _state.Claims = _state
            .Claims
            .Where(p => p.ClaimType != claim.Type || p.ClaimValue != claim.Value)
            .Union([new ApplicationUserClaim
            {
                UserId = Id.ToUnescapeString(),
                ClaimType = newClaim.ValueType,
                ClaimValue = newClaim.Value,
            }
            ]);

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
        ArgumentNullException.ThrowIfNull(user);
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
            ConditionalValue<UserActorState> result = await StateManager.TryGetStateAsync<UserActorState>(DaprIdentityStoreConstants.UserIdentityStateName, cancellationToken);
            if (result.HasValue)
            {
                _state = result.Value;
            }
        }

        return _state;
    }
}