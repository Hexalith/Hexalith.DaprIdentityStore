// <copyright file="CustomRoleClaimTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Security.Claims;
using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomRoleClaim"/> class.
/// </summary>
public class CustomRoleClaimTests
{
    /// <summary>
    /// Verifies that the initialize from claim method
    /// correctly sets the properties of the <see cref="CustomRoleClaim"/> instance
    /// based on the provided <see cref="Claim"/>.
    /// </summary>
    [Fact]
    public void CustomRoleClaim_InitializeFromClaim_ShouldSetProperties()
    {
        // Arrange: Create a Claim instance with test data.
        Claim claim = new(ClaimTypes.Role, "Admin");
        CustomRoleClaim roleClaim = new();

        // Act: Initialize the CustomRoleClaim instance from the Claim.
        roleClaim.InitializeFromClaim(claim);

        // Assert: Verify that the properties are set correctly.
        roleClaim.ClaimType.ShouldBe(ClaimTypes.Role);
        roleClaim.ClaimValue.ShouldBe("Admin");
    }

    /// <summary>
    /// Verifies that the properties of the <see cref="CustomRoleClaim"/> instance
    /// are correctly set during initialization.
    /// </summary>
    [Fact]
    public void CustomRoleClaim_Properties_ShouldBeSetCorrectly()
    {
        // Arrange: Create an instance of CustomRoleClaim with test data.
        CustomRoleClaim roleClaim = new()
        {
            Id = 1,
            RoleId = "role123",
            ClaimType = ClaimTypes.Role,
            ClaimValue = "Admin",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert: Verify that all properties are set correctly.
        roleClaim.Id.ShouldBe(1);
        roleClaim.RoleId.ShouldBe("role123");
        roleClaim.ClaimType.ShouldBe(ClaimTypes.Role);
        roleClaim.ClaimValue.ShouldBe("Admin");
        roleClaim.ExternalData.ShouldBe("{\"key\":\"value\"}");
        roleClaim.ExternalId.ShouldBe("ext123");
    }

    /// <summary>
    /// Verifies that serialization and deserialization using System.Text.Json
    /// maintains the properties of the <see cref="CustomRoleClaim"/> instance.
    /// </summary>
    [Fact]
    public void CustomRoleClaim_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create an instance of CustomRoleClaim with test data.
        CustomRoleClaim originalRoleClaim = new()
        {
            Id = 1,
            RoleId = "role123",
            ClaimType = ClaimTypes.Role,
            ClaimValue = "Admin",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act: Serialize and then deserialize the instance using System.Text.Json.
        string serializedRoleClaim = JsonSerializer.Serialize(originalRoleClaim);
        CustomRoleClaim deserializedRoleClaim = JsonSerializer.Deserialize<CustomRoleClaim>(serializedRoleClaim);

        // Assert: Verify that the deserialized instance matches the original.
        _ = deserializedRoleClaim.ShouldNotBeNull();
        deserializedRoleClaim.Id.ShouldBe(originalRoleClaim.Id);
        deserializedRoleClaim.RoleId.ShouldBe(originalRoleClaim.RoleId);
        deserializedRoleClaim.ClaimType.ShouldBe(originalRoleClaim.ClaimType);
        deserializedRoleClaim.ClaimValue.ShouldBe(originalRoleClaim.ClaimValue);
        deserializedRoleClaim.ExternalData.ShouldBe(originalRoleClaim.ExternalData);
        deserializedRoleClaim.ExternalId.ShouldBe(originalRoleClaim.ExternalId);
    }

    /// <summary>
    /// Verifies that the to claim method
    /// returns a <see cref="Claim"/> instance with the correct properties.
    /// </summary>
    [Fact]
    public void CustomRoleClaim_ToClaim_ShouldReturnCorrectClaim()
    {
        // Arrange: Create an instance of CustomRoleClaim with test data.
        CustomRoleClaim roleClaim = new()
        {
            ClaimType = ClaimTypes.Role,
            ClaimValue = "Admin",
        };

        // Act: Convert the CustomRoleClaim instance to a Claim.
        Claim claim = roleClaim.ToClaim();

        // Assert: Verify that the Claim properties match the original data.
        claim.Type.ShouldBe(ClaimTypes.Role);
        claim.Value.ShouldBe("Admin");
    }
}