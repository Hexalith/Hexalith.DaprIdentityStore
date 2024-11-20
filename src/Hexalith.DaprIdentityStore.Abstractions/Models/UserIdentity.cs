// <copyright file="UserIdentity.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user identity in the Dapr identity store.
/// Extends the base IdentityUser class to provide core user identity functionality.
/// This class serves as the primary user entity for authentication and user management.
/// </summary>
public class UserIdentity : IdentityUser
{
    /// <summary>
    /// Gets or sets the contact identifier associated with the user.
    /// </summary>
    /// <value>
    /// The contact identifier associated with the user.
    /// </value>
    public string? ContactId { get; set; }
}