// <copyright file="IdentityStoreExtensions - Copy.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Helpers;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.UI.Account;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

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
        => services
            .AddDaprIdentityStore()
            .AddCascadingAuthenticationState()
            .AddScoped<IdentityUserAccessor>()
            .AddScoped<IdentityRedirectManager>()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
}