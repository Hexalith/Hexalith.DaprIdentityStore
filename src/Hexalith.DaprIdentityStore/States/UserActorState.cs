// <copyright file="UserActorState.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.States;

using System.Runtime.Serialization;

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
[DataContract]
public class UserActorState
{
    /// <summary>
    /// Gets or sets the claims associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="CustomUserClaim"/> representing the user's claims.
    /// </value>
    public IEnumerable<CustomUserClaim> Claims { get; set; } = [];

    /// <summary>
    /// Gets or sets the logins associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="UserLoginInfo"/> representing the user's logins.
    /// </value>
    public IEnumerable<CustomUserLoginInfo> Logins { get; set; } = [];

    /// <summary>
    /// Gets or sets the tokens associated with the user.
    /// </summary>
    /// <value>
    /// A collection of <see cref="CustomUserToken"/> representing the user's tokens.
    /// </value>
    public IEnumerable<CustomUserToken> Tokens { get; set; } = [];

    /// <summary>
    /// Gets or sets the user identity information.
    /// </summary>
    /// <value>
    /// An instance of <see cref="CustomUser"/> containing the user's identity details.
    /// Defaults to a new instance if not explicitly set.
    /// </value>
    /// <remarks>
    /// This property stores core user identity information such as user credentials,
    /// profile data, and authentication details.
    /// </remarks>
    public CustomUser User { get; set; } = new();
}