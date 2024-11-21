// <copyright file="DaprActorRoleStore.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Stores;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
/// </summary>
/// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe identity errors.</param>
public class DaprActorRoleStore(IdentityErrorDescriber describer) : RoleStoreBase<CustomRole, string, CustomUserRole, CustomRoleClaim>(describer)
{
    /// <inheritdoc/>
    public override IQueryable<CustomRole> Roles => Array.Empty<CustomRole>().AsQueryable();

    /// <inheritdoc/>
    public override Task AddClaimAsync(CustomRole role, Claim claim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<CustomRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<CustomRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<IList<Claim>> GetClaimsAsync(CustomRole role, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task RemoveClaimAsync(CustomRole role, Claim claim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}