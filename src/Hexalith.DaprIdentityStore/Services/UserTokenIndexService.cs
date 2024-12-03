// <copyright file="UserTokenIndexService.cs" company="ITANEO">
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
/// Service for managing user identity token indices.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserTokenIndexService"/> class.
/// </remarks>
/// <param name="factory">The actor proxy factory.</param>
public class UserTokenIndexService(IActorProxyFactory factory) : IUserTokenIndexService
{
    // Factory function to create key-value actors for login indexing
    private readonly Func<string, string, IKeyValueActor> _keyValueActor = factory.CreateUserTokenIndexProxy;

    /// <inheritdoc/>
    public Task AddAsync(string loginProvider, string name, string userId)
    {
        ArgumentNullException.ThrowIfNull(loginProvider);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(userId);
        return _keyValueActor(loginProvider, name).SetAsync(userId);
    }

    /// <inheritdoc/>
    public Task<string?> FindUserIdAsync(string loginProvider, string name)
    {
        ArgumentNullException.ThrowIfNull(loginProvider);
        ArgumentNullException.ThrowIfNull(name);
        return _keyValueActor(loginProvider, name).GetAsync();
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string loginProvider, string name)
    {
        ArgumentNullException.ThrowIfNull(loginProvider);
        ArgumentNullException.ThrowIfNull(name);
        return _keyValueActor(loginProvider, name).RemoveAsync();
    }
}