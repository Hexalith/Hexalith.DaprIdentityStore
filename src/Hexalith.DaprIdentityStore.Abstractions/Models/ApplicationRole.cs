// <copyright file="ApplicationRole.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an application role in the Dapr identity store.
/// Extends the base IdentityRole class to provide role-based authorization capabilities.
/// </summary>
#pragma warning disable S2094 // Classes should not be empty

public class ApplicationRole : IdentityRole
#pragma warning restore S2094 // Classes should not be empty
{
}