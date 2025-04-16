// <copyright file="CustomUserLoginTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System.Text.Json;

using Hexalith.DaprIdentityStore.Models;

using Shouldly;

/// <summary>
/// Unit tests for the <see cref="CustomUserLogin"/> class.
/// </summary>
public class CustomUserLoginTests
{
    /// <summary>
    /// Tests that the default constructor of <see cref="CustomUserLogin"/> creates a valid instance.
    /// </summary>
    [Fact]
    public void CustomUserLogin_DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act: Create an instance of CustomUserLogin using the default constructor.
        CustomUserLogin userLogin = new();

        // Assert: Verify that the instance is not null.
        _ = userLogin.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that the parameterized constructor of <see cref="CustomUserLogin"/> correctly sets properties.
    /// </summary>
    [Fact]
    public void CustomUserLogin_ParameterizedConstructor_ShouldSetProperties()
    {
        // Arrange: Prepare the input data for the parameterized constructor.
        CustomUserLoginInfo loginInfo = new("TestProvider", "ProviderKey123", "DisplayName");
        string userId = "User123";

        // Act: Create an instance of CustomUserLogin using the parameterized constructor.
        CustomUserLogin userLogin = new(loginInfo, userId);

        // Assert: Verify that the properties are set correctly.
        userLogin.LoginProvider.ShouldBe("TestProvider");
        userLogin.ProviderKey.ShouldBe("ProviderKey123");
        userLogin.ProviderDisplayName.ShouldBe("DisplayName");
        userLogin.UserId.ShouldBe("User123");
    }

    /// <summary>
    /// Tests that the properties of <see cref="CustomUserLogin"/> can be set and retrieved correctly.
    /// </summary>
    [Fact]
    public void CustomUserLogin_Properties_ShouldBeSetCorrectly()
    {
        // Arrange: Create an instance of CustomUserLogin and set its properties.
        CustomUserLogin userLogin = new()
        {
            LoginProvider = "TestProvider",
            ProviderKey = "ProviderKey123",
            ProviderDisplayName = "DisplayName",
            UserId = "User123",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Assert: Verify that all properties are set correctly.
        userLogin.LoginProvider.ShouldBe("TestProvider");
        userLogin.ProviderKey.ShouldBe("ProviderKey123");
        userLogin.ProviderDisplayName.ShouldBe("DisplayName");
        userLogin.UserId.ShouldBe("User123");
        userLogin.ExternalData.ShouldBe("{\"key\":\"value\"}");
        userLogin.ExternalId.ShouldBe("ext123");
    }

    /// <summary>
    /// Tests that a <see cref="CustomUserLogin"/> instance can be serialized to JSON and deserialized back
    /// while maintaining all property values.
    /// </summary>
    [Fact]
    public void CustomUserLogin_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create an instance of CustomUserLogin with specific properties.
        CustomUserLogin originalUserLogin = new()
        {
            LoginProvider = "TestProvider",
            ProviderKey = "ProviderKey123",
            ProviderDisplayName = "DisplayName",
            UserId = "User123",
            ExternalData = "{\"key\":\"value\"}",
            ExternalId = "ext123",
        };

        // Act: Serialize the instance to JSON and then deserialize it back.
        string serializedUserLogin = JsonSerializer.Serialize(originalUserLogin);
        CustomUserLogin deserializedUserLogin = JsonSerializer.Deserialize<CustomUserLogin>(serializedUserLogin);

        // Assert: Verify that the deserialized instance matches the original instance.
        _ = deserializedUserLogin.ShouldNotBeNull();
        deserializedUserLogin.LoginProvider.ShouldBe(originalUserLogin.LoginProvider);
        deserializedUserLogin.ProviderKey.ShouldBe(originalUserLogin.ProviderKey);
        deserializedUserLogin.ProviderDisplayName.ShouldBe(originalUserLogin.ProviderDisplayName);
        deserializedUserLogin.UserId.ShouldBe(originalUserLogin.UserId);
        deserializedUserLogin.ExternalData.ShouldBe(originalUserLogin.ExternalData);
        deserializedUserLogin.ExternalId.ShouldBe(originalUserLogin.ExternalId);
    }
}