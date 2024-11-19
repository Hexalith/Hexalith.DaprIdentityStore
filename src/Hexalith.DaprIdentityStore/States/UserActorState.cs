// <copyright file="UserActorState.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.States;

using Hexalith.DaprIdentityStore.Models;

/// <summary>
/// Represents the state of a user actor in the Dapr identity store.
/// Contains the user's identity information.
/// </summary>
public class UserActorState
{
    /// <summary>
    /// Gets or sets the user identity information.
    /// </summary>
    public UserIdentity User { get; set; } = new();
}