// <copyright file="DaprActorRoleStore{Roles}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
/// </summary>
/// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe identity errors.</param>
public partial class DaprActorRoleStore(IdentityErrorDescriber describer) : RoleStoreBase<CustomRole, string, CustomUserRole, CustomRoleClaim>(describer)
{
    private readonly IRoleCollectionService? _roleIdentityCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
    /// </summary>
    /// <param name="roleIdentityCollection"></param>
    /// <param name="describer"></param>
    public DaprActorRoleStore(
        IRoleCollectionService roleIdentityCollection,
        IdentityErrorDescriber? describer = null)
        : this(describer ?? new IdentityErrorDescriber())
    {
        ArgumentNullException.ThrowIfNull(roleIdentityCollection);
        _roleIdentityCollection = roleIdentityCollection;
    }

    /// <inheritdoc/>
    public override IQueryable<CustomRole> Roles
    {
        get
        {
            ThrowIfDisposed();
            return GetRolesAsync().GetAwaiter().GetResult().AsQueryable();
        }
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        bool created = await actor.CreateAsync(role);
        return created
            ? IdentityResult.Success
            : IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.Name));
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.DeleteAsync();
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public override async Task<CustomRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(roleId);
        return await actor.FindAsync();
    }

    /// <inheritdoc/>
    public override async Task<CustomRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedRoleName);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateRoleNameIndexProxy(normalizedRoleName);
        string? roleId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(roleId) ? null : await FindByIdAsync(roleId, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (await FindByIdAsync(role.Id, cancellationToken) == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Role '{role.Id}' not found.",
            });
        }

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.UpdateAsync(role);
        return IdentityResult.Success;
    }

    private async Task<List<CustomRole>> GetRolesAsync()
    {
        ThrowIfDisposed();

        IKeyHashActor allProxy = ActorProxy.DefaultProxyFactory.CreateAllRolesProxy();
        IEnumerable<string> roleIds = await allProxy.AllAsync();
        List<Task<CustomRole?>> tasks = [];
        foreach (string roleId in roleIds)
        {
            IRoleActor roleProxy = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(roleId);
            tasks.Add(roleProxy.FindAsync());
        }

        return (await Task.WhenAll(tasks))
            .Where(p => p != null)
            .OfType<CustomRole>()
            .ToList();
    }
}