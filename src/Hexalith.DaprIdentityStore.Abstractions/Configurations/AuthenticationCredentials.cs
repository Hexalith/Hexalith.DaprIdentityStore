// <copyright file="AuthenticationCredentials.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Configurations;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the authentication credentials.
/// </summary>
[DataContract]
public record class AuthenticationCredentials(
    [property: DataMember(Order = 1)] string Id,
    [property: DataMember(Order = 2)] string Secret)
{
    /// <summary>
    /// Gets an empty instance of <see cref="AuthenticationCredentials"/>.
    /// </summary>
    public static AuthenticationCredentials Empty => new(string.Empty, string.Empty);

    /// <summary>
    /// Gets a value indicating whether the credentials are enabled.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public bool Enabled => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Secret);
}