// <copyright file="RoleActorState.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.States;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Represents the state of a role actor in the Dapr identity store.
/// This class maintains the role's identity information and state within the Dapr actor system.
/// </summary>
/// <remarks>
/// The RoleActorState is used by Dapr actors to persist role-related information
/// across actor invocations. It serves as the primary state container for role data
/// in the distributed actor system.
/// </remarks>
internal class RoleActorState
{
    /// <summary>
    /// Gets or sets the claims associated with the role.
    /// </summary>
    /// <value>
    /// A collection of <see cref="CustomRoleClaim"/> representing the role's claims.
    /// </value>
    internal IEnumerable<CustomRoleClaim> Claims { get; set; } = [];

    /// <summary>
    /// Gets or sets the role identity information.
    /// </summary>
    /// <value>
    /// An instance of <see cref="CustomRole"/> containing the role's identity details.
    /// Defaults to a new instance if not explicitly set.
    /// </value>
    /// <remarks>
    /// This property stores core role identity information such as role credentials,
    /// profile data, and authentication details.
    /// </remarks>
    internal CustomRole Role { get; set; } = new();
}