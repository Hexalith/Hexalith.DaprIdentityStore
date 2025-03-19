// <copyright file="IdentityStoreMapExtensions.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Helpers;

using System.Security.Claims;
using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.UI.Account.Pages.Login;
using Hexalith.DaprIdentityStore.UI.Account.Pages.Manage;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Provides extension methods for mapping additional identity endpoints required by the Identity Razor components.
/// </summary>
public static class IdentityStoreMapExtensions
{
    /// <summary>
    /// Maps additional identity endpoints required by the Identity Razor components.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint convention builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the endpoints parameter is null.</exception>
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        RouteGroupBuilder accountGroup = endpoints.MapGroup("/Account");

        _ = accountGroup.MapPost("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<CustomUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return Results.BadRequest("Invalid provider.");
            }

            provider = provider.Split(",").First();
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("ReturnUrl", returnUrl),
                new("Action", ExternalLogin.LoginCallbackAction)];

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/ExternalLogin",
                QueryString.Create(query));

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        _ = accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<CustomUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            if (user.Identity?.IsAuthenticated == true)
            {
                await signInManager.SignOutAsync();
            }

            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        RouteGroupBuilder manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        _ = manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<CustomUser> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/Manage/ExternalLogins",
                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
            return TypedResults.Challenge(properties, [provider]);
        });

        ILoggerFactory loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        ILogger downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        _ = manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<CustomUser> userManager,
            [FromServices] AuthenticationStateProvider _) =>
        {
            CustomUser? user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            string userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            Dictionary<string, string> personalData = [];
            IEnumerable<System.Reflection.PropertyInfo> personalDataProps = typeof(CustomUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (System.Reflection.PropertyInfo? p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
            foreach (UserLoginInfo l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            byte[] fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            return context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json")
                ? TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json")
                : TypedResults.LocalRedirect($"~/");
        });

        return accountGroup;
    }
}