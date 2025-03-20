// <copyright file="DaprIdentityStoreSettings.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Configurations;

using System.Runtime.Serialization;

using Hexalith.Extensions.Configuration;

/// <summary>
/// Security settings.
/// </summary>
[DataContract]
public class DaprIdentityStoreSettings(
    [property: DataMember(Order = 1)] AuthenticationCredentials? microsoft,
    [property: DataMember(Order = 2)] AuthenticationCredentials? github,
    [property: DataMember(Order = 3)] AuthenticationCredentials? google,
    [property: DataMember(Order = 4)] AuthenticationCredentials? facebook,
    [property: DataMember(Order = 5)] AuthenticationCredentials? x) : ISettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DaprIdentityStoreSettings"/> class.
    /// </summary>
    public DaprIdentityStoreSettings()
        : this(null, null, null, null, null)
    {
    }

    /// <summary>
    /// Gets the Facebook authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Facebook { get; } = facebook;

    /// <summary>
    /// Gets the GitHub authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Github { get; } = github;

    /// <summary>
    /// Gets the Google authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Google { get; } = google;

    /// <summary>
    /// Gets the Microsoft authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Microsoft { get; } = microsoft;

    /// <summary>
    /// Gets the X authentication credentials.
    /// </summary>
    public AuthenticationCredentials? X { get; } = x;

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(DaprIdentityStore);
}