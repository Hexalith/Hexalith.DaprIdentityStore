// <copyright file="CustomRoleTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomRole"/> class.
/// </summary>
public class CustomRoleTests
{
    /// <summary>
    /// Verifies that the properties of the <see cref="CustomRole"/> class are set correctly.
    /// </summary>
    [Fact]
    public void CustomRole_Properties_ShouldBeSetCorrectly()
    {
        // Arrange: Create a new CustomRole object with predefined properties
        CustomRole role = new()
        {
            Id = "role123",
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "stamp123",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert: Verify that all properties are set correctly using Shouldly
        role.Id.ShouldBe("role123"); // Check if Id is set correctly
        role.Name.ShouldBe("Admin"); // Check if Name is set correctly
        role.NormalizedName.ShouldBe("ADMIN"); // Check if NormalizedName is set correctly
        role.ConcurrencyStamp.ShouldBe("stamp123"); // Check if ConcurrencyStamp is set correctly
        role.ExternalData.ShouldBe("{\"key\":\"value\"}"); // Check if ExternalData is set correctly
        role.ExternalId.ShouldBe("ext123"); // Check if ExternalId is set correctly
    }

    /// <summary>
    /// Verifies that the <see cref="CustomRole"/> class can be serialized and deserialized while maintaining its properties.
    /// </summary>
    [Fact]
    public void CustomRole_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create a new CustomRole object with predefined properties
        CustomRole originalRole = new()
        {
            Id = "role123",
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "stamp123",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act: Serialize the object to JSON and then deserialize it back
        string serializedRole = JsonSerializer.Serialize(originalRole);
        CustomRole deserializedRole = JsonSerializer.Deserialize<CustomRole>(serializedRole);

        // Assert: Verify that the deserialized object matches the original object
        _ = deserializedRole.ShouldNotBeNull(); // Ensure the deserialized object is not null
        deserializedRole.Id.ShouldBe(originalRole.Id); // Check if Id matches
        deserializedRole.Name.ShouldBe(originalRole.Name); // Check if Name matches
        deserializedRole.NormalizedName.ShouldBe(originalRole.NormalizedName); // Check if NormalizedName matches
        deserializedRole.ConcurrencyStamp.ShouldBe(originalRole.ConcurrencyStamp); // Check if ConcurrencyStamp matches
        deserializedRole.ExternalData.ShouldBe(originalRole.ExternalData); // Check if ExternalData matches
        deserializedRole.ExternalId.ShouldBe(originalRole.ExternalId); // Check if ExternalId matches
    }
}