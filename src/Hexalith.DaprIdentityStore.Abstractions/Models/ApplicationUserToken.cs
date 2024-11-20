// <copyright file="ApplicationUserToken.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an authentication token for a user in the Dapr identity store.
/// Extends IdentityUserToken with string-based user identifiers to store authentication tokens.
/// </summary>
public class ApplicationUserToken : IdentityUserToken<string>
{
}