// <copyright file="UserPartitionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application.Sessions.Services;
using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Service for managing user partitions.
/// </summary>
public class UserPartitionService : IUserPartitionService
{
    private readonly IUserStore<CustomUser> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPartitionService"/> class.
    /// </summary>
    /// <param name="userStore">The user store.</param>
    public UserPartitionService(IUserStore<CustomUser> userStore)
    {
        ArgumentNullException.ThrowIfNull(userStore);
        _userStore = userStore;
    }

    /// <inheritdoc/>
    public async Task<string?> GetDefaultPartitionAsync(string userId, CancellationToken cancellationToken)
    {
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken);
        return user is null ? null : user.DefaultPartition ?? user.Partitions.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetPartitionsAsync(string userId, CancellationToken cancellationToken)
    {
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken);
        return user?.Partitions ?? [];
    }

    /// <inheritdoc/>
    public async Task<bool> InPartitionAsync(string userId, string partitionId, CancellationToken cancellationToken)
    {
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken);
        return user?.Partitions.Contains(partitionId) ?? false;
    }
}