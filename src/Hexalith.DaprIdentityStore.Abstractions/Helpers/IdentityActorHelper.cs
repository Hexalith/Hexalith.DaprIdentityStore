// <copyright file="IdentityActorHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

/// <summary>
/// Provides helper methods for identity actors.
/// </summary>
public static class IdentityActorHelper
{
    /// <summary>
    /// Formats a claim type and value into a standardized claim identifier string.
    /// </summary>
    /// <param name="claimType">The type of the claim.</param>
    /// <param name="claimValue">The value of the claim.</param>
    /// <returns>A formatted string combining the claim type and value with a separator.</returns>
    /// <exception cref="ArgumentException">Thrown when claimType is null or whitespace.</exception>
    public static string ToClaimId([NotNull] string claimType, string claimValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        return $"{claimType}-{claimValue}";
    }

    /// <summary>
    /// Converts a Claim object into a standardized claim identifier string.
    /// </summary>
    /// <param name="claim">The claim object to convert.</param>
    /// <returns>A formatted string combining the claim's type and value with a separator.</returns>
    /// <exception cref="ArgumentNullException">Thrown when claim is null.</exception>
    public static string ToClaimId([NotNull] this Claim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        return ToClaimId(claim.Type, claim.Value);
    }

    /// <summary>
    /// Formats a login provider and provider key into a standardized login identifier string.
    /// </summary>
    /// <param name="loginProvider">The name of the login provider (e.g., "Google", "Facebook").</param>
    /// <param name="providerKey">The provider-specific key for the user.</param>
    /// <returns>A formatted string combining the login provider and provider key with a separator.</returns>
    /// <exception cref="ArgumentException">Thrown when loginProvider or providerKey is null or whitespace.</exception>
    public static string ToLoginId([NotNull] string loginProvider, [NotNull] string providerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        return $"{loginProvider}-{providerKey}";
    }
}