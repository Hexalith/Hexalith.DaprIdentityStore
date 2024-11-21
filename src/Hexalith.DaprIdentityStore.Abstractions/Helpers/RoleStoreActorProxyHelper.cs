// <copyright file="RoleStoreActorProxyHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Provides helper methods for creating actor proxies used in Dapr identity store operations.
/// This static class simplifies the creation of various actor proxies needed for role management.
/// </summary>
public static class RoleStoreActorProxyHelper
{
    /// <summary>
    /// Creates a proxy for the actor that manages the collection of all roles.
    /// This actor maintains a hash set of all role IDs in the system.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the all roles collection actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory is null.</exception>
    /// <remarks>
    /// The returned actor proxy can be used to add, remove, or query role IDs from the global collection.
    /// </remarks>
    public static IKeyHashActor CreateAllRolesProxy([NotNull] this IActorProxyFactory actorProxyFactory)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        return actorProxyFactory.CreateActorProxy<IKeyHashActor>(
            DaprIdentityStoreConstants.AllRolesCollectionActorId.ToActorId(),
            DaprIdentityStoreConstants.RoleCollectionActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the role claim index.
    /// This actor maintains a mapping between claim type/value combinations and role IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="claim">The claim object containing type and value to index.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the role claim index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or claim is null.</exception>
    /// <remarks>
    /// The claim index actor enables efficient role lookups by claim information, supporting role-based and claim-based authorization scenarios.
    /// </remarks>
    public static IKeyHashActor CreateClaimRolesIndexProxy([NotNull] this IActorProxyFactory actorProxyFactory, [NotNull] Claim claim)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        return actorProxyFactory.CreateClaimRolesIndexProxy(claim.ToClaimId());
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the role claim index using separate claim type and value.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="claimType">The type of the claim.</param>
    /// <param name="claimValue">The value of the claim.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the role claim index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or claimType is null.</exception>
    public static IKeyHashActor CreateClaimRolesIndexProxy([NotNull] this IActorProxyFactory actorProxyFactory, [NotNull] string claimType, string claimValue)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        return actorProxyFactory.CreateClaimRolesIndexProxy(IdentityActorHelper.ToClaimId(claimType, claimValue));
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the role claim index using a pre-formatted claim ID.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="id">The pre-formatted claim identifier.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the role claim index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory is null.</exception>
    /// <exception cref="ArgumentException">Thrown when id is null or whitespace.</exception>
    public static IKeyHashActor CreateClaimRolesIndexProxy([NotNull] this IActorProxyFactory actorProxyFactory, [NotNull] string id)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return actorProxyFactory.CreateActorProxy<IKeyHashActor>(id.ToActorId(), DaprIdentityStoreConstants.RoleClaimIndexActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages individual role identity data.
    /// This actor handles all operations related to a specific role's identity information.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="id">The unique identifier of the role.</param>
    /// <returns>A proxy implementing IRoleIdentityActor interface to interact with the role identity actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or id is null.</exception>
    /// <remarks>
    /// The role identity actor manages role-specific data including claims, roles, and authentication information.
    /// </remarks>
    public static IRoleActor CreateRoleIdentityActor([NotNull] this IActorProxyFactory actorProxyFactory, [NotNull] string id)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return actorProxyFactory.CreateActorProxy<IRoleActor>(id.ToActorId(), DaprIdentityStoreConstants.DefaultRoleActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the Role index.
    /// This actor maintains a mapping between normalized Roles and role IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="normalizedName">The normalized Role used as the actor identifier.</param>
    /// <returns>A proxy implementing IKeyValueActor interface to interact with the Role index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or normalizedName is null.</exception>
    /// <remarks>
    /// The Role index actor ensures Role uniqueness across the system and provides quick role lookup by Role.
    /// </remarks>
    public static IKeyValueActor CreateRoleNameIndexProxy([NotNull] this IActorProxyFactory actorProxyFactory, [NotNull] string normalizedName)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedName);
        return actorProxyFactory.CreateActorProxy<IKeyValueActor>(normalizedName.ToActorId(), DaprIdentityStoreConstants.RoleNameIndexActorTypeName);
    }
}