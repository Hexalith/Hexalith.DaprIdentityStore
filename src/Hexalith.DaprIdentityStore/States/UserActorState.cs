// <copyright file="UserActorState.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.States;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents the state of a user actor in the Dapr identity store.
/// This class maintains the user's identity information and state within the Dapr actor system.
/// </summary>
/// <remarks>
/// The UserActorState is used by Dapr actors to persist user-related information
/// across actor invocations. It serves as the primary state container for user data
/// in the distributed actor system.
/// </remarks>
internal class UserActorState
{
    /// <summary>
    /// Gets or sets the claims associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="ApplicationUserClaim"/> representing the user's claims.
    /// </value>
    internal IEnumerable<ApplicationUserClaim> Claims { get; set; } = [];

    /// <summary>
    /// Gets or sets the logins associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="UserLoginInfo"/> representing the user's logins.
    /// </value>
    internal IEnumerable<ApplicationUserLogin> Logins { get; set; } = [];

    /// <summary>
    /// Gets or sets the tokens associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="ApplicationUserToken"/> representing the user's tokens.
    /// </value>
    internal IEnumerable<ApplicationUserToken> Tokens { get; set; } = [];

    /// <summary>
    /// Gets or sets the user identity information.
    /// </summary>
    /// <value>
    /// An instance of <see cref="UserIdentity"/> containing the user's identity details.
    /// Defaults to a new instance if not explicitly set.
    /// </value>
    /// <remarks>
    /// This property stores core user identity information such as user credentials,
    /// profile data, and authentication details.
    /// </remarks>
    internal UserIdentity User { get; set; } = new();
}