// <copyright file="CustomUserLoginInfo.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents login information and source for a user record.
/// </summary>
[DataContract]
public record CustomUserLoginInfo(
    [property: DataMember(Order = 1)] string LoginProvider,
    [property: DataMember(Order = 2)] string ProviderKey,
    [property: DataMember(Order = 3)] string? DisplayName)
{
    /// <summary>
    /// Gets the <see cref="UserLoginInfo"/> instance.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public UserLoginInfo UserLoginInfo => new(LoginProvider, ProviderKey, DisplayName);

    /// <summary>
    /// Creates a new instance of <see cref="CustomUserLoginInfo"/> from a <see cref="UserLoginInfo"/> instance.
    /// </summary>
    /// <param name="info">The <see cref="UserLoginInfo"/> instance to convert.</param>
    /// <returns>A new <see cref="CustomUserLoginInfo"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="info"/> is null.</exception>
    public static CustomUserLoginInfo Create(UserLoginInfo info)
        => new(
              (info ?? throw new ArgumentNullException(nameof(info))).LoginProvider,
              info.ProviderKey,
              info.ProviderDisplayName);
}