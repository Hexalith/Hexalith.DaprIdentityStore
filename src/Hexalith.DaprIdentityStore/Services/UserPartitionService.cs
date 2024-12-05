// <copyright file="UserPartitionService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application.Partitions.Services;
using Hexalith.Application.Sessions.Services;
using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Service for managing user partitions.
/// </summary>
public class UserPartitionService : IUserPartitionService
{
    private readonly IPartitionService _partitionService;
    private readonly UserManager<CustomUser> _userManager;
    private readonly IUserStore<CustomUser> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPartitionService"/> class.
    /// </summary>
    /// <param name="userStore">The user store.</param>
    /// <param name="partitionService">The partition service.</param>
    /// <param name="userManager">The user manager.</param>
    public UserPartitionService(
        IUserStore<CustomUser> userStore,
        IPartitionService partitionService,
        UserManager<CustomUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(userStore);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(partitionService);
        _userStore = userStore;
        _partitionService = partitionService;
        _userManager = userManager;
    }

    /// <inheritdoc/>
    public async Task<string> GetDefaultPartitionAsync(string userName, CancellationToken cancellationToken)
    {
        CustomUser user = await GetUserAsync(userName, cancellationToken);
        string? partition = user.DefaultPartition ?? user.Partitions.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(partition))
        {
            partition = await _partitionService.DefaultAsync(cancellationToken);
            user.DefaultPartition = partition;
            user.Partitions = [partition];
            _ = await _userStore.UpdateAsync(user, cancellationToken);
        }

        return partition;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetPartitionsAsync(string userName, CancellationToken cancellationToken)
    {
        CustomUser user = await GetUserAsync(userName, cancellationToken);
        if (user.Partitions.Any())
        {
            return user.Partitions;
        }

        string partition = await _partitionService.DefaultAsync(cancellationToken);
        user.DefaultPartition = partition;
        user.Partitions = [partition];
        _ = await _userStore.UpdateAsync(user, cancellationToken);

        return user.Partitions;
    }

    /// <inheritdoc/>
    public async Task<bool> InPartitionAsync(string userName, string partitionId, CancellationToken cancellationToken)
    {
        IEnumerable<string> partitions = await GetPartitionsAsync(userName, cancellationToken);
        return partitions.Any(p => p == partitionId);
    }

    private async Task<CustomUser> GetUserAsync(string userName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        string normalized = _userManager.NormalizeName(userName)
            ?? throw new InvalidOperationException($"User with name '{userName}' has an empty normalized name.");
        return await _userStore.FindByNameAsync(normalized, cancellationToken)
            ?? throw new InvalidOperationException($"User with name '{userName}' (Normalized:{normalized}) not found.");
    }
}