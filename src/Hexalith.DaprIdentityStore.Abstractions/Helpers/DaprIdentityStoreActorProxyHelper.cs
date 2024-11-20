// <copyright file="DaprIdentityStoreActorProxyHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Helpers;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Provides helper methods for creating actor proxies used in Dapr identity store operations.
/// This static class simplifies the creation of various actor proxies needed for user management.
/// </summary>
public static class DaprIdentityStoreActorProxyHelper
{
    /// <summary>
    /// Creates a proxy for the actor that manages the collection of all users.
    /// This actor maintains a hash set of all user IDs in the system.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the all users collection actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory is null.</exception>
    /// <remarks>
    /// The returned actor proxy can be used to add, remove, or query user IDs from the global collection.
    /// </remarks>
    public static IKeyHashActor CreateAllUsersProxy(this IActorProxyFactory actorProxyFactory)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        return actorProxyFactory.CreateActorProxy<IKeyHashActor>(DaprIdentityStoreConstants.AllUsersCollectionActorId.ToActorId(), DaprIdentityStoreConstants.UserCollectionActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the user email index.
    /// This actor maintains a mapping between normalized email addresses and user IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="normalizedEmail">The normalized email address used as the actor identifier.</param>
    /// <returns>A proxy implementing IKeyValueActor interface to interact with the user email index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or normalizedEmail is null.</exception>
    /// <remarks>
    /// The email index actor ensures email uniqueness across the system and provides quick user lookup by email.
    /// </remarks>
    public static IKeyValueActor CreateUserEmailIndexProxy(this IActorProxyFactory actorProxyFactory, string normalizedEmail)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);
        return actorProxyFactory.CreateActorProxy<IKeyValueActor>(normalizedEmail.ToActorId(), DaprIdentityStoreConstants.UserEmailIndexActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages individual user identity data.
    /// This actor handles all operations related to a specific user's identity information.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A proxy implementing IUserIdentityActor interface to interact with the user identity actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or id is null.</exception>
    /// <remarks>
    /// The user identity actor manages user-specific data including claims, roles, and authentication information.
    /// </remarks>
    public static IUserIdentityActor CreateUserIdentityActor(this IActorProxyFactory actorProxyFactory, string id)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return actorProxyFactory.CreateActorProxy<IUserIdentityActor>(id.ToActorId(), DaprIdentityStoreConstants.DefaultUserActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the username index.
    /// This actor maintains a mapping between normalized usernames and user IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="normalizedName">The normalized username used as the actor identifier.</param>
    /// <returns>A proxy implementing IKeyValueActor interface to interact with the username index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory or normalizedName is null.</exception>
    /// <remarks>
    /// The username index actor ensures username uniqueness across the system and provides quick user lookup by username.
    /// </remarks>
    public static IKeyValueActor CreateUserNameIndexProxy(this IActorProxyFactory actorProxyFactory, string normalizedName)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedName);
        return actorProxyFactory.CreateActorProxy<IKeyValueActor>(normalizedName.ToActorId(), DaprIdentityStoreConstants.UserNameIndexActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the user login index.
    /// This actor maintains a mapping between login provider/key combinations and user IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="providerKey">The provider-specific key for the user.</param>
    /// <returns>A proxy implementing IKeyValueActor interface to interact with the user login index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory, loginProvider, or providerKey is null.</exception>
    /// <remarks>
    /// The login index actor enables quick user lookup by external login information, supporting third-party authentication scenarios.
    /// </remarks>
    public static IKeyValueActor CreateUserLoginIndexProxy(this IActorProxyFactory actorProxyFactory, string loginProvider, string providerKey)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        string actorId = $"{loginProvider}:{providerKey}";
        return actorProxyFactory.CreateActorProxy<IKeyValueActor>(actorId.ToActorId(), DaprIdentityStoreConstants.UserLoginIndexActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the user claim index.
    /// This actor maintains a mapping between claim type/value combinations and user IDs.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="claimType">The type of the claim.</param>
    /// <param name="claimValue">The value of the claim.</param>
    /// <returns>A proxy implementing IKeyHashActor interface to interact with the user claim index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory, claimType, or claimValue is null.</exception>
    /// <remarks>
    /// The claim index actor enables efficient user lookups by claim information, supporting role-based and claim-based authorization scenarios.
    /// </remarks>
    public static IKeyHashActor CreateUserClaimIndexProxy(this IActorProxyFactory actorProxyFactory, string claimType, string claimValue)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimValue);
        string actorId = $"{claimType}:{claimValue}";
        return actorProxyFactory.CreateActorProxy<IKeyHashActor>(actorId.ToActorId(), DaprIdentityStoreConstants.UserClaimIndexActorTypeName);
    }

    /// <summary>
    /// Creates a proxy for the actor that manages the user token index.
    /// This actor maintains a mapping between login provider/name combinations and user tokens.
    /// </summary>
    /// <param name="actorProxyFactory">The actor proxy factory used to create actor proxies.</param>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="name">The name of the token.</param>
    /// <returns>A proxy implementing IKeyValueActor interface to interact with the user token index actor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when actorProxyFactory, loginProvider, or name is null.</exception>
    /// <remarks>
    /// The token index actor enables management of user-specific tokens for various authentication and authorization scenarios.
    /// </remarks>
    public static IKeyValueActor CreateUserTokenIndexProxy(this IActorProxyFactory actorProxyFactory, string loginProvider, string name)
    {
        ArgumentNullException.ThrowIfNull(actorProxyFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        string actorId = $"{loginProvider}:{name}";
        return actorProxyFactory.CreateActorProxy<IKeyValueActor>(actorId.ToActorId(), DaprIdentityStoreConstants.UserTokenIndexActorTypeName);
    }
}