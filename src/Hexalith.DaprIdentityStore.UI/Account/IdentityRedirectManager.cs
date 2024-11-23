// <copyright file="IdentityRedirectManager.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Account;

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

#pragma warning disable S3994 // URI Parameters should not be strings

/// <summary>
/// Manages redirection within the identity store UI.
/// </summary>
internal sealed class IdentityRedirectManager
{
    /// <summary>
    /// The name of the status message cookie.
    /// </summary>
    internal const string _statusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder _statusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityRedirectManager"/> class.
    /// </summary>
    /// <param name="navigationManager">The navigation manager.</param>
    internal IdentityRedirectManager(NavigationManager navigationManager) => _navigationManager = navigationManager;

    private string CurrentPath => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    /// <exception cref="InvalidOperationException">Thrown when used outside of static rendering.</exception>
    [DoesNotReturn]
    internal void RedirectTo(string? uri)
    {
        uri ??= string.Empty;

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = _navigationManager.ToBaseRelativePath(uri);
        }

        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        _navigationManager.NavigateTo(uri);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    /// <summary>
    /// Redirects to the specified URI with query parameters.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    /// <param name="queryParameters">The query parameters to include in the URI.</param>
    /// <exception cref="InvalidOperationException">Thrown when used outside of static rendering.</exception>
    [DoesNotReturn]
    internal void RedirectTo(string uri, IReadOnlyDictionary<string, object?> queryParameters)
    {
        string uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        string newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }

    /// <summary>
    /// Redirects to the current page.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when used outside of static rendering.</exception>
    [DoesNotReturn]
    internal void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    /// <summary>
    /// Redirects to the current page with a status message.
    /// </summary>
    /// <param name="message">The status message to include.</param>
    /// <param name="context">The HTTP context.</param>
    /// <exception cref="InvalidOperationException">Thrown when used outside of static rendering.</exception>
    [DoesNotReturn]
    internal void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        => RedirectToWithStatus(CurrentPath, message, context);

    /// <summary>
    /// Redirects to the specified URI with a status message.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    /// <param name="message">The status message to include.</param>
    /// <param name="context">The HTTP context.</param>
    /// <exception cref="InvalidOperationException">Thrown when used outside of static rendering.</exception>
    [DoesNotReturn]
    internal void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(_statusCookieName, message, _statusCookieBuilder.Build(context));
        RedirectTo(uri);
    }
}