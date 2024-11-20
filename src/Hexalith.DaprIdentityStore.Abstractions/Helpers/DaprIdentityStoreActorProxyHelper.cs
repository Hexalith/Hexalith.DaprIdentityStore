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
}