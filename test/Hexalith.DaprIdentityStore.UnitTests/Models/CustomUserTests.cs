// <copyright file="CustomUserTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomUser"/> class.
/// Verifies that the properties are set correctly and serialization/deserialization maintains data integrity.
/// </summary>
public class CustomUserTests
{
    /// <summary>
    /// Tests that the properties of <see cref="CustomUser"/> are set correctly.
    /// </summary>
    [Fact]
    public void CustomUser_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        CustomUser user = new()
        {
            Id = "user123",
            UserName = "testuser",
            Email = "test@example.com",
            DefaultPartition = "partition1",
            Disabled = false,
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
            Partitions = ["partition1", "partition2"],
        };

        // Assert
        user.Id.ShouldBe("user123");
        user.UserName.ShouldBe("testuser");
        user.Email.ShouldBe("test@example.com");
        user.DefaultPartition.ShouldBe("partition1");
        user.Disabled.ShouldBeFalse();
        user.ExternalData.ShouldBe("{\"key\":\"value\"}");
        user.ExternalId.ShouldBe("ext123");
        user.Partitions.Count().ShouldBe(2);
        user.Partitions.ShouldContain("partition1");
        user.Partitions.ShouldContain("partition2");
    }

    /// <summary>
    /// Tests that serialization and deserialization of <see cref="CustomUser"/> maintains property values.
    /// </summary>
    [Fact]
    public void CustomUser_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange
        CustomUser originalUser = new()
        {
            Id = "user123",
            UserName = "testuser",
            Email = "test@example.com",
            DefaultPartition = "partition1",
            Disabled = false,
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
            Partitions = ["partition1", "partition2"],
        };

        // Act
        string serializedUser = JsonSerializer.Serialize(originalUser);
        CustomUser deserializedUser = JsonSerializer.Deserialize<CustomUser>(serializedUser);

        // Assert
        _ = deserializedUser.ShouldNotBeNull();
        deserializedUser.Id.ShouldBe(originalUser.Id);
        deserializedUser.UserName.ShouldBe(originalUser.UserName);
        deserializedUser.Email.ShouldBe(originalUser.Email);
        deserializedUser.DefaultPartition.ShouldBe(originalUser.DefaultPartition);
        deserializedUser.Disabled.ShouldBe(originalUser.Disabled);
        deserializedUser.ExternalData.ShouldBe(originalUser.ExternalData);
        deserializedUser.ExternalId.ShouldBe(originalUser.ExternalId);
        deserializedUser.Partitions.Count().ShouldBe(originalUser.Partitions.Count());
        foreach (string partition in originalUser.Partitions)
        {
            deserializedUser.Partitions.ShouldContain(partition);
        }
    }
}