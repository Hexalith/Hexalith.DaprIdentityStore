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
    private readonly UserManager<CustomUser> _userManager;
    private readonly IUserStore<CustomUser> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPartitionService"/> class.
    /// </summary>
    /// <param name="userStore">The user store.</param>
    /// <param name="userManager">The user manager.</param>
    public UserPartitionService(IUserStore<CustomUser> userStore, UserManager<CustomUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(userStore);
        ArgumentNullException.ThrowIfNull(userManager);
        _userStore = userStore;
        _userManager = userManager;
    }

    /// <inheritdoc/>
    public async Task<string?> GetDefaultPartitionAsync(string userName, CancellationToken cancellationToken)
    {
        CustomUser? user = await FindUserAsync(userName, cancellationToken);
        return user is null ? null : user.DefaultPartition ?? user.Partitions.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetPartitionsAsync(string userName, CancellationToken cancellationToken)
    {
        CustomUser? user = await FindUserAsync(userName, cancellationToken);
        return user?.Partitions ?? [];
    }

    /// <inheritdoc/>
    public async Task<bool> InPartitionAsync(string userName, string partitionId, CancellationToken cancellationToken)
    {
        CustomUser? user = await FindUserAsync(userName, cancellationToken);
        return user?.Partitions.Contains(partitionId) ?? false;
    }

    private async Task<CustomUser> FindUserAsync(string userName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        string normalized = _userManager.NormalizeName(userName)
            ?? throw new InvalidOperationException($"User with name '{userName}' has an empty normalized name.");
        return await _userStore.FindByNameAsync(normalized, cancellationToken)
            ?? throw new InvalidOperationException($"User with name '{userName}' (Normalized:{normalized}) not found.");
    }
}