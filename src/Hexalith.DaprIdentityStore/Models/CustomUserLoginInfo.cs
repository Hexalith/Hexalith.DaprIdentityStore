// <copyright file="CustomUserLoginInfo.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

using Newtonsoft.Json;

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
    /// Initializes a new instance of the <see cref="CustomUserLoginInfo"/> class.
    /// </summary>
    /// <param name="info">The <see cref="UserLoginInfo"/> to copy.</param>
    public CustomUserLoginInfo(UserLoginInfo info)
        : this(
              (info ?? throw new ArgumentNullException(nameof(info))).LoginProvider,
              info.ProviderKey,
              info.ProviderDisplayName)
    {
    }

    /// <summary>
    /// Gets the <see cref="UserLoginInfo"/> instance.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public UserLoginInfo UserLoginInfo => new(LoginProvider, ProviderKey, DisplayName);
}