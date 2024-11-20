// <copyright file="HexalithIdentityErrorDescriber.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Errors;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides error descriptions for identity operations in Hexalith.
/// </summary>
public class HexalithIdentityErrorDescriber : IdentityErrorDescriber
{
    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a duplicate user ID.
    /// </summary>
    /// <param name="userId">The user ID that is duplicated.</param>
    /// <returns>An <see cref="IdentityError"/> indicating a duplicate user ID.</returns>
    public IdentityError DuplicateUserId(string userId) => new()
    {
        Code = nameof(DuplicateUserId),
        Description = $"User id '{userId}' is already taken.",
    };
}