// <copyright file="CustomRole.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an application role in the Dapr identity store.
/// Extends the base IdentityRole class to provide role-based authorization capabilities.
/// </summary>
[DataContract]
public class CustomRole : IdentityRole
#pragma warning restore S2094 // Classes should not be empty
{
    /// <summary>
    /// Gets or sets the external data associated with the role.
    /// </summary>
    public string? ExternalData { get; set; }

    /// <summary>
    /// Gets or sets the external identifier associated with the role.
    /// </summary>
    public string? ExternalId { get; set; }
}