// <copyright file="CustomUserClaimTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Security.Claims;
using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomUserClaim"/> class.
/// </summary>
public class CustomUserClaimTests
{
    /// <summary>
    /// Tests that the <see cref="CustomUserClaim.InitializeFromClaim"/> method sets properties correctly.
    /// </summary>
    [Fact]
    public void CustomUserClaim_InitializeFromClaim_ShouldSetProperties()
    {
        // Arrange: Create a Claim and an empty CustomUserClaim.
        Claim claim = new(ClaimTypes.Name, "Test User");
        CustomUserClaim userClaim = new();

        // Act: Initialize the CustomUserClaim from the Claim.
        userClaim.InitializeFromClaim(claim);

        // Assert: Verify that the properties of CustomUserClaim are set correctly.
        userClaim.ClaimType.ShouldBe(ClaimTypes.Name);
        userClaim.ClaimValue.ShouldBe("Test User");
    }

    /// <summary>
    /// Tests that the properties of <see cref="CustomUserClaim"/> are set correctly.
    /// </summary>
    [Fact]
    public void CustomUserClaim_Properties_ShouldBeSetCorrectly()
    {
        // Arrange: Create a new instance of CustomUserClaim with test data.
        CustomUserClaim userClaim = new()
        {
            Id = 1,
            UserId = "user123",
            ClaimType = ClaimTypes.Name,
            ClaimValue = "Test User",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert: Verify that all properties are set as expected using Shouldly.
        userClaim.Id.ShouldBe(1);
        userClaim.UserId.ShouldBe("user123");
        userClaim.ClaimType.ShouldBe(ClaimTypes.Name);
        userClaim.ClaimValue.ShouldBe("Test User");
        userClaim.ExternalData.ShouldBe("{\"key\":\"value\"}");
        userClaim.ExternalId.ShouldBe("ext123");
    }

    /// <summary>
    /// Tests that serializing and deserializing a <see cref="CustomUserClaim"/> maintains its properties.
    /// </summary>
    [Fact]
    public void CustomUserClaim_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create a new instance of CustomUserClaim with test data.
        CustomUserClaim originalUserClaim = new()
        {
            Id = 1,
            UserId = "user123",
            ClaimType = ClaimTypes.Name,
            ClaimValue = "Test User",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act: Serialize and then deserialize the CustomUserClaim.
        string serializedUserClaim = JsonSerializer.Serialize(originalUserClaim);
        CustomUserClaim deserializedUserClaim = JsonSerializer.Deserialize<CustomUserClaim>(serializedUserClaim);

        // Assert: Verify that the deserialized object matches the original.
        _ = deserializedUserClaim.ShouldNotBeNull();
        deserializedUserClaim.Id.ShouldBe(originalUserClaim.Id);
        deserializedUserClaim.UserId.ShouldBe(originalUserClaim.UserId);
        deserializedUserClaim.ClaimType.ShouldBe(originalUserClaim.ClaimType);
        deserializedUserClaim.ClaimValue.ShouldBe(originalUserClaim.ClaimValue);
        deserializedUserClaim.ExternalData.ShouldBe(originalUserClaim.ExternalData);
        deserializedUserClaim.ExternalId.ShouldBe(originalUserClaim.ExternalId);
    }

    /// <summary>
    /// Tests that the <see cref="CustomUserClaim.ToClaim"/> method returns the correct <see cref="Claim"/>.
    /// </summary>
    [Fact]
    public void CustomUserClaim_ToClaim_ShouldReturnCorrectClaim()
    {
        // Arrange: Create a new instance of CustomUserClaim with test data.
        CustomUserClaim userClaim = new()
        {
            ClaimType = ClaimTypes.Name,
            ClaimValue = "Test User",
        };

        // Act: Convert the CustomUserClaim to a Claim.
        Claim claim = userClaim.ToClaim();

        // Assert: Verify that the Claim's properties match the expected values.
        claim.Type.ShouldBe(ClaimTypes.Name);
        claim.Value.ShouldBe("Test User");
    }
}