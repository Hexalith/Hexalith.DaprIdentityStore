// <copyright file="DaprIdentityStoreHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Helpers;

using System;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides helper methods for partition actors.
/// </summary>
public static class DaprIdentityStoreHelper
{
    /// <summary>
    /// Adds Dapr identity store services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with the added services.</returns>
    public static IServiceCollection AddDaprIdentityStore(this IServiceCollection services)
    {
        _ = services.AddSingleton<IUserIdentityCollectionService, UserIdentityCollectionService>();
        _ = services.AddSingleton<IUserIdentityNameCollectionService, UserIdentityNameCollectionService>();
        _ = services.AddSingleton<IUserIdentityEmailCollectionService, UserIdentityEmailCollectionService>();
        return services;
    }

    /// <summary>
    /// Registers partition actors with the specified ActorRegistrationCollection.
    /// </summary>
    /// <param name="actorRegistrationCollection">The ActorRegistrationCollection to register actors with.</param>
    /// <exception cref="ArgumentNullException">Thrown when actorRegistrationCollection is null.</exception>
    public static void RegisterPartitionActors(this ActorRegistrationCollection actorRegistrationCollection)
    {
        ArgumentNullException.ThrowIfNull(actorRegistrationCollection);
        actorRegistrationCollection.RegisterActor<UserIdentityActor>(DaprIdentityStoreConstants.DefaultUserActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(DaprIdentityStoreConstants.UserCollectionActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserEmailIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserNameIndexActorTypeName);
    }
}