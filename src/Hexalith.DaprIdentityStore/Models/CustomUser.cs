// <copyright file="CustomUser.cs" company="ITANEO">
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
public class CustomUser : IdentityUser
{
    /// <summary>
    /// Gets or sets a value indicating whether the user is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the external data associated with the user.
    /// This can be used to store additional information from external systems.
    /// </summary>
    public string? ExternalData { get; set; }

    /// <summary>
    /// Gets or sets the external identifier for the user.
    /// This can be used to link the user to an external system.
    /// </summary>
    public string? ExternalId { get; set; }
}