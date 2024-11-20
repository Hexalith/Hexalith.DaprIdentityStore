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
    : UserStoreBase<UserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    private readonly IUserIdentityCollectionService _userIdentityCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaprActorUserStore"/> class.
    /// </summary>
    /// <param name="userIdentityCollection">The user identity collection service.</param>
    public DaprActorUserStore(IUserIdentityCollectionService userIdentityCollection)
        : base(new HexalithIdentityErrorDescriber())
    {
        ArgumentNullException.ThrowIfNull(userIdentityCollection);
        _userIdentityCollection = userIdentityCollection;
    }

    /// <inheritdoc/>
#pragma warning disable S4462 // Calls to "async" methods should not be blocking
    public override IQueryable<UserIdentity> Users => GetUsersAsync().GetAwaiter().GetResult().AsQueryable();
#pragma warning restore S4462 // Calls to "async" methods should not be blocking

    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        bool created = await actor.CreateAsync(user);
        return created ? IdentityResult.Success : IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.NormalizedUserName ?? "Unknown"));
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.DeleteAsync();
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public override async Task<UserIdentity?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(normalizedEmail);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<UserIdentity?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
        return await actor.FindAsync();
    }

    /// <inheritdoc/>
    public override async Task<UserIdentity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUserName);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(normalizedUserName);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (await FindByIdAsync(user.Id, cancellationToken) == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = $"A user with the Id '{user.Id}' could not be found." });
        }

        IUserIdentityActor actor = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(user.Id);
        await actor.UpdateAsync(user);
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    protected override async Task<UserIdentity?> FindUserAsync(string userId, CancellationToken cancellationToken)
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
    private async Task<List<UserIdentity>> GetUsersAsync()
    {
        ThrowIfDisposed();

        IKeyHashActor allProxy = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        IEnumerable<string> userIds = await allProxy.AllAsync();
        List<Task<UserIdentity?>> tasks = [];
        foreach (string userId in userIds)
        {
            IUserIdentityActor userProxy = ActorProxy.DefaultProxyFactory.CreateUserIdentityActor(userId);
            tasks.Add(userProxy.FindAsync());
        }

        return (await Task.WhenAll(tasks)).Where(p => p != null).OfType<UserIdentity>().ToList();
    }
}