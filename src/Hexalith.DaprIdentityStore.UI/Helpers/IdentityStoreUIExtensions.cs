// <copyright file="IdentityStoreUIExtensions.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Helpers;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Stores;
using Hexalith.DaprIdentityStore.UI.Account;
using Hexalith.DaprIdentityStore.UI.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Provides extension methods for mapping additional identity endpoints required by the Identity Razor components.
/// </summary>
public static class IdentityStoreUIExtensions
{
    /// <summary>
    /// Adds the Dapr Identity Store UI services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The IServiceCollection with the services added.</returns>
    public static IServiceCollection AddDaprIdentityStoreUI(this IServiceCollection services)
    {
        services.TryAddScoped<IUserClaimStore<CustomUser>, DaprActorUserStore>();
        services.TryAddScoped<IRoleClaimStore<CustomRole>, DaprActorRoleStore>();
        services.TryAddScoped<IUserStore<CustomUser>, DaprActorUserStore>();
        services.TryAddScoped<IRoleStore<CustomRole>, DaprActorRoleStore>();
        _ = services
            .AddDaprIdentityStoreServer()
            .AddScoped<IEmailSender<CustomUser>, EmailSender>()
            .AddScoped<IdentityUserAccessor>()
            .AddScoped<IdentityRedirectManager>()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddIdentity<CustomUser, CustomRole>()
            .AddRoles<CustomRole>()
            .AddDefaultTokenProviders();
        return services;
    }
}