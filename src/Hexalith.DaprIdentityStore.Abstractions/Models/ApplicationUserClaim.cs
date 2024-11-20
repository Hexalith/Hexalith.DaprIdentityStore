// <copyright file="ApplicationUserClaim.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a claim that belongs to a user in the Dapr identity store.
/// Extends IdentityUserClaim with string-based user identifiers.
/// </summary>
public class ApplicationUserClaim : IdentityUserClaim<string>
{
}