// <copyright file="CustomUserClaim.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a claim that belongs to a user in the Dapr identity store.
/// Extends IdentityUserClaim with string-based user identifiers.
/// </summary>
public class CustomUserClaim : IdentityUserClaim<string>
{
    /// <summary>
    /// Gets or sets the external data associated with the user claim.
    /// </summary>
    public string? ExternalData { get; set; }

    /// <summary>
    /// Gets or sets the external identifier associated with the user claim.
    /// </summary>
    public string? ExternalId { get; set; }
}