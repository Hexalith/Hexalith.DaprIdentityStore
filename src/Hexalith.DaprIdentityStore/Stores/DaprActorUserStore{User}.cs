// <copyright file="DaprActorUserStore{User}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Errors;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user store that uses Dapr actors for user management.
/// </summary>
public partial class DaprActorUserStore
    : UserStoreBase<CustomUser, string, CustomUserClaim, CustomUserLogin, CustomUserToken>
{
    private readonly IUserLoginIndexService _loginCollectionService;
    private readonly IUserCollectionService _userCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaprActorUserStore"/> class.
    /// </summary>
    /// <param name="userCollection">The user identity collection service.</param>
    /// <param name="loginCollectionService">The login collection service.</param>
    public DaprActorUserStore(
        IUserCollectionService userCollection,
        IUserLoginIndexService loginCollectionService)
        : base(new HexalithIdentityErrorDescriber())
    {
        ArgumentNullException.ThrowIfNull(userCollection);
        ArgumentNullException.ThrowIfNull(loginCollectionService);
        _userCollection = userCollection;
        _loginCollectionService = loginCollectionService;
    }

    /// <inheritdoc/>
#pragma warning disable S4462 // Calls to "async" methods should not be blocking
    public override IQueryable<CustomUser> Users => GetUsersAsync().GetAwaiter().GetResult().AsQueryable();
#pragma warning restore S4462 // Calls to "async" methods should not be blocking

    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        bool created = await actor.CreateAsync(user);
        return created ? IdentityResult.Success : IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.NormalizedUserName ?? "Unknown"));
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        await actor.DeleteAsync();
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public override async Task<CustomUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(normalizedEmail);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CustomUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(userId);
        return await actor.FindAsync();
    }

    /// <inheritdoc/>
    public override async Task<CustomUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUserName);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(normalizedUserName);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (await FindByIdAsync(user.Id, cancellationToken) == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = $"A user with the Id '{user.Id}' could not be found." });
        }

        IUserActor actor = ActorProxy.DefaultProxyFactory.CreateUserActor(user.Id);
        await actor.UpdateAsync(user);
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    protected override async Task<CustomUser?> FindUserAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return await FindByIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Gets the list of all users asynchronously.
    /// </summary>
    /// <returns>A list of all users.</returns>
    private async Task<List<CustomUser>> GetUsersAsync()
    {
        ThrowIfDisposed();
        IEnumerable<string> userIds = await _userCollection.AllAsync();
        List<Task<CustomUser?>> tasks = [];
        foreach (string userId in userIds)
        {
            IUserActor userProxy = ActorProxy.DefaultProxyFactory.CreateUserActor(userId);
            tasks.Add(userProxy.FindAsync());
        }

        return (await Task.WhenAll(tasks)).Where(p => p != null).OfType<CustomUser>().ToList();
    }
}