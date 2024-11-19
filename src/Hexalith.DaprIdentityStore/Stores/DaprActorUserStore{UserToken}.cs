// <copyright file="DaprActorUserStore{UserToken}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System.Threading;
using System.Threading.Tasks;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user store that uses Dapr actors for user management.
/// </summary>
public partial class DaprActorUserStore
    : UserStoreBase<UserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    /// <inheritdoc/>
    protected override Task AddUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override Task<ApplicationUserToken?> FindTokenAsync(UserIdentity user, string loginProvider, string name, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override Task RemoveUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();
}