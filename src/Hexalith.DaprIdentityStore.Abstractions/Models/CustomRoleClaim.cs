// <copyright file="CustomRoleClaim.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a custom role claim with additional external data and ID.
/// </summary>
[DataContract]
public class CustomRoleClaim : IdentityRoleClaim<string>
#pragma warning restore S2094 // Classes should not be empty
{
    /// <summary>
    /// Gets or sets the external data associated with the role claim.
    /// </summary>
    public string? ExternalData { get; set; }

    /// <summary>
    /// Gets or sets the external ID associated with the role claim.
    /// </summary>
    public string? ExternalId { get; set; }
}