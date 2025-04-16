// <copyright file="CustomUserTokenTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomUserToken"/> class.
/// Verifies that the properties are set correctly and serialization/deserialization works as expected.
/// </summary>
public class CustomUserTokenTests
{
    /// <summary>
    /// Tests that the properties of <see cref="CustomUserToken"/> are set correctly.
    /// </summary>
    [Fact]
    public void CustomUserToken_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        CustomUserToken userToken = new()
        {
            UserId = "user123",
            LoginProvider = "TestProvider",
            Name = "TokenName",
            Value = "TokenValue",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert
        userToken.UserId.ShouldBe("user123");
        userToken.LoginProvider.ShouldBe("TestProvider");
        userToken.Name.ShouldBe("TokenName");
        userToken.Value.ShouldBe("TokenValue");
        userToken.ExternalData.ShouldBe("{\"key\":\"value\"}");
        userToken.ExternalId.ShouldBe("ext123");
    }

    /// <summary>
    /// Tests that serialization and deserialization of <see cref="CustomUserToken"/> maintain the property values.
    /// </summary>
    [Fact]
    public void CustomUserToken_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange
        CustomUserToken originalUserToken = new()
        {
            UserId = "user123",
            LoginProvider = "TestProvider",
            Name = "TokenName",
            Value = "TokenValue",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act
        string serializedUserToken = JsonSerializer.Serialize(originalUserToken);
        CustomUserToken deserializedUserToken = JsonSerializer.Deserialize<CustomUserToken>(serializedUserToken);

        // Assert
        _ = deserializedUserToken.ShouldNotBeNull();
        deserializedUserToken.UserId.ShouldBe(originalUserToken.UserId);
        deserializedUserToken.LoginProvider.ShouldBe(originalUserToken.LoginProvider);
        deserializedUserToken.Name.ShouldBe(originalUserToken.Name);
        deserializedUserToken.Value.ShouldBe(originalUserToken.Value);
        deserializedUserToken.ExternalData.ShouldBe(originalUserToken.ExternalData);
        deserializedUserToken.ExternalId.ShouldBe(originalUserToken.ExternalId);
    }
}