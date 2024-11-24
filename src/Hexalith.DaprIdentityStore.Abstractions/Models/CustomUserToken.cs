// <copyright file="CustomUserToken.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an authentication token for a user in the Dapr identity store.
/// Extends IdentityUserToken with string-based user identifiers to store authentication tokens.
/// </summary>
[DataContract]
public class CustomUserToken : IdentityUserToken<string>
{
    /// <summary>
    /// Gets or sets the external data associated with the token.
    /// </summary>
    public string? ExternalData { get; set; }

    /// <summary>
    /// Gets or sets the external identifier associated with the token.
    /// </summary>
    public string? ExternalId { get; set; }
}