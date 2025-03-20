// <copyright file="DaprIdentityStoreSettings.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Configurations;

using System.Runtime.Serialization;

/// <summary>
/// Security settings.
/// </summary>
[DataContract]
public record DaprIdentityStoreSettings(
    [property: DataMember(Order = 1)] AuthenticationCredentials Microsoft,
    [property: DataMember(Order = 2)] AuthenticationCredentials Github,
    [property: DataMember(Order = 3)] AuthenticationCredentials Google,
    [property: DataMember(Order = 4)] AuthenticationCredentials Facebook,
    [property: DataMember(Order = 5)] AuthenticationCredentials X,
    [property: DataMember(Order = 7)] bool Disabled)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DaprIdentityStoreSettings"/> class.
    /// </summary>
    public DaprIdentityStoreSettings()
        : this(
              AuthenticationCredentials.Empty,
              AuthenticationCredentials.Empty,
              AuthenticationCredentials.Empty,
              AuthenticationCredentials.Empty,
              AuthenticationCredentials.Empty,
              false)
    {
    }

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(DaprIdentityStore);
}