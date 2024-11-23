// <copyright file="DaprIdentityStoreHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Helpers;

using System;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.DaprIdentityStore.Stores;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        _ = services
            .AddControllers()
                .AddDapr();
        services.TryAddSingleton<IUserCollectionService, UserCollectionService>();
        services.TryAddSingleton<IUserNameIndexService, UserNameIndexService>();
        services.TryAddSingleton<IUserEmailIndexService, UserEmailIndexService>();
        services.TryAddSingleton<IUserLoginIndexService, UserLoginIndexService>();
        services.TryAddSingleton<IUserClaimsIndexService, UserClaimsIndexService>();
        services.TryAddSingleton<IUserTokenIndexService, UserTokenIndexService>();
        services.TryAddSingleton<IRoleCollectionService, RoleCollectionService>();
        services.TryAddSingleton<IRoleNameIndexService, RoleNameIndexService>();
        services.TryAddSingleton<IRoleClaimsIndexService, RoleClaimsIndexService>();
        _ = services.AddIdentity<CustomUser, CustomRole>()

            // .AddRoles<CustomRole>()
            // .AddRoleValidator<RoleValidator<CustomRole>>()
            // .AddRoleManager<RoleManager<CustomRole>>()
            // .AddPasswordValidator<DaprActorRoleStore>()
            // .AddUserValidator<DaprActorRoleStore>()
            // .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<CustomUser, CustomRole>>()
            // .AddUserManager<UserManager<CustomUser>>()
            .AddDefaultTokenProviders()
            .AddRoleStore<DaprActorRoleStore>()
            .AddUserStore<DaprActorUserStore>();

        // _ = services.AddScoped<DaprActorUserStore>();
        // _ = services.AddScoped<IUserClaimStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserLoginStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserAuthenticationTokenStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserPasswordStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserSecurityStampStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserEmailStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserLockoutStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserPhoneNumberStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IQueryableUserStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserTwoFactorStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserAuthenticatorKeyStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserTwoFactorRecoveryCodeStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        // _ = services.AddScoped<IUserLoginStore<CustomUser>>(services => services.GetRequiredService<DaprActorUserStore>());
        return services;
    }

    /// <summary>
    /// Registers partition actors with the specified ActorRegistrationCollection.
    /// </summary>
    /// <param name="actorRegistrationCollection">The ActorRegistrationCollection to register actors with.</param>
    /// <exception cref="ArgumentNullException">Thrown when actorRegistrationCollection is null.</exception>
    public static void RegisterIdentityActors(this ActorRegistrationCollection actorRegistrationCollection)
    {
        ArgumentNullException.ThrowIfNull(actorRegistrationCollection);
        actorRegistrationCollection.RegisterActor<UserActor>(DaprIdentityStoreConstants.DefaultUserActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(DaprIdentityStoreConstants.UserCollectionActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserEmailIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserNameIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserLoginIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(DaprIdentityStoreConstants.UserClaimIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.UserTokenIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<RoleActor>(DaprIdentityStoreConstants.DefaultRoleActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(DaprIdentityStoreConstants.RoleCollectionActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(DaprIdentityStoreConstants.RoleClaimIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(DaprIdentityStoreConstants.RoleNameIndexActorTypeName);
    }
}