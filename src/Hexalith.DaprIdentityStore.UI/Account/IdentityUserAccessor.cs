// <copyright file="IdentityUserAccessor.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Account;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides access to the identity user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IdentityUserAccessor"/> class.
/// </remarks>
/// <param name="userManager">The user manager.</param>
/// <param name="redirectManager">The redirect manager.</param>
public sealed class IdentityUserAccessor(UserManager<CustomUser> userManager, IdentityRedirectManager redirectManager)
{
    private readonly IdentityRedirectManager _redirectManager = redirectManager;
    private readonly UserManager<CustomUser> _userManager = userManager;

    /// <summary>
    /// Gets the required user asynchronously.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The custom user.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user cannot be loaded.</exception>
    public async Task<CustomUser> GetRequiredUserAsync(HttpContext context)
    {
        CustomUser? user = await _userManager.GetUserAsync(context.User);

        if (user is null)
        {
            _redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{_userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}