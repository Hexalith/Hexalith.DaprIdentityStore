// <copyright file="DaprActorUserStore{UserLogin}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
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
    public override Task AddLoginAsync(UserIdentity user, UserLoginInfo login, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<IList<UserLoginInfo>> GetLoginsAsync(UserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task RemoveLoginAsync(UserIdentity user, string loginProvider, string providerKey, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();
}