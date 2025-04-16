// <copyright file="CustomUserRoleTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomUserRole"/> class.
/// Verifies that the properties are set correctly and that serialization/deserialization maintains data integrity.
/// </summary>
public class CustomUserRoleTests
{
    /// <summary>
    /// Tests that the properties of <see cref="CustomUserRole"/> are set correctly.
    /// </summary>
    [Fact]
    public void CustomUserRole_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        CustomUserRole userRole = new()
        {
            UserId = "user123",
            RoleId = "role456",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert
        userRole.UserId.ShouldBe("user123");
        userRole.RoleId.ShouldBe("role456");
        userRole.ExternalData.ShouldBe("{\"key\":\"value\"}");
        userRole.ExternalId.ShouldBe("ext123");
    }

    /// <summary>
    /// Tests that serialization and deserialization of <see cref="CustomUserRole"/> maintain the integrity of the properties.
    /// </summary>
    [Fact]
    public void CustomUserRole_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange
        CustomUserRole originalUserRole = new()
        {
            UserId = "user123",
            RoleId = "role456",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act
        string serializedUserRole = JsonSerializer.Serialize(originalUserRole);
        CustomUserRole deserializedUserRole = JsonSerializer.Deserialize<CustomUserRole>(serializedUserRole);

        // Assert
        _ = deserializedUserRole.ShouldNotBeNull();
        deserializedUserRole.UserId.ShouldBe(originalUserRole.UserId);
        deserializedUserRole.RoleId.ShouldBe(originalUserRole.RoleId);
        deserializedUserRole.ExternalData.ShouldBe(originalUserRole.ExternalData);
        deserializedUserRole.ExternalId.ShouldBe(originalUserRole.ExternalId);
    }
}