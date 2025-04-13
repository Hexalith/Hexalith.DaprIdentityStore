// <copyright file="DaprIdentityStoreSettings.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Configurations;

using Hexalith.Commons.Configurations;

/// <summary>
/// Security settings.
/// </summary>
public class DaprIdentityStoreSettings(
    AuthenticationCredentials? microsoft,
    AuthenticationCredentials? github,
    AuthenticationCredentials? google,
    AuthenticationCredentials? facebook,
    AuthenticationCredentials? x) : ISettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DaprIdentityStoreSettings"/> class.
    /// </summary>
    public DaprIdentityStoreSettings()
        : this(null, null, null, null, null)
    {
    }

    /// <summary>
    /// Gets or sets the Facebook authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Facebook { get; set; } = facebook;

    /// <summary>
    /// Gets or sets the GitHub authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Github { get; set; } = github;

    /// <summary>
    /// Gets or sets the Google authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Google { get; set; } = google;

    /// <summary>
    /// Gets or sets the Microsoft authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Microsoft { get; set; } = microsoft;

    /// <summary>
    /// Gets or sets the X authentication credentials.
    /// </summary>
    public AuthenticationCredentials? X { get; set; } = x;

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(DaprIdentityStore);
}