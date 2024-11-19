namespace Hexalith.DaprIdentityStore.Helpers;

using System;

using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Provides helper methods for partition actors.
/// </summary>
public static class DaprIdentityStoreHelper
{
    public static IKeyHashActor CreateAllUsersProxy(this IActorProxyFactory actorProxyFactory)
        => actorProxyFactory.CreateActorProxy<IKeyHashActor>(UserIdentityActor.AllCollectionId.ToActorId(), UserIdentityActor.ActorCollectionTypeName);

    /// <summary>
    /// Registers partition actors with the specified ActorRegistrationCollection.
    /// </summary>
    /// <param name="actorRegistrationCollection">The ActorRegistrationCollection to register actors with.</param>
    /// <exception cref="ArgumentNullException">Thrown when actorRegistrationCollection is null.</exception>
    public static void RegisterPartitionActors(this ActorRegistrationCollection actorRegistrationCollection)
    {
        ArgumentNullException.ThrowIfNull(actorRegistrationCollection);
        actorRegistrationCollection.RegisterActor<UserIdentityActor>(UserIdentityActor.DefaultActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(UserIdentityActor.ActorCollectionTypeName);
    }
}