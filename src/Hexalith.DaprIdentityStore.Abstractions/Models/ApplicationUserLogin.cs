// <copyright file="ApplicationUserLogin.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user's login information in the Dapr identity store.
/// Extends IdentityUserLogin with string-based user identifiers to store external login provider data.
/// </summary>
public class ApplicationUserLogin : IdentityUserLogin<string>
{
}