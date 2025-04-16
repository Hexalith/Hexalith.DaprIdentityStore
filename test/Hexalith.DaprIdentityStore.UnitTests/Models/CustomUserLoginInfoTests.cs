// <copyright file="CustomUserLoginInfoTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Models;

using System;

using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

using Newtonsoft.Json;

using Shouldly;

using JsonSerializer = System.Text.Json.JsonSerializer;

/// <summary>
/// Unit tests for the <see cref="CustomUserLoginInfo"/> class.
/// </summary>
public class CustomUserLoginInfoTests
{
    /// <summary>
    /// Verifies that the <see cref="CustomUserLoginInfo.Create"/> method creates an instance
    /// with the correct properties based on the provided <see cref="UserLoginInfo"/>.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_Create_ShouldReturnCorrectInstance()
    {
        // Arrange: Create a UserLoginInfo instance with test data.
        UserLoginInfo userLoginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Act: Use the Create method to create a CustomUserLoginInfo instance.
        CustomUserLoginInfo customLoginInfo = CustomUserLoginInfo.Create(userLoginInfo);

        // Assert: Verify that the properties of the created instance match the input data.
        customLoginInfo.LoginProvider.ShouldBe("TestProvider");
        customLoginInfo.ProviderKey.ShouldBe("ProviderKey123");
        customLoginInfo.DisplayName.ShouldBe("DisplayName");
    }

    /// <summary>
    /// Verifies that the <see cref="CustomUserLoginInfo.Create"/> method throws an
    /// <see cref="ArgumentNullException"/> when the input is null.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_Create_ShouldThrowExceptionForNullInput() =>

        // Act & Assert: Ensure an exception is thrown when null is passed.
        Should.Throw<ArgumentNullException>(() => CustomUserLoginInfo.Create(null));

    /// <summary>
    /// Tests that serialization and deserialization using Newtonsoft.Json
    /// maintains the properties of the <see cref="CustomUserLoginInfo"/> instance.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_JsonNet_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create an instance of CustomUserLoginInfo with test data.
        CustomUserLoginInfo originalLoginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Act: Serialize and then deserialize the instance using Newtonsoft.Json.
        string serializedLoginInfo = JsonConvert.SerializeObject(originalLoginInfo);
        CustomUserLoginInfo deserializedLoginInfo = JsonConvert.DeserializeObject<CustomUserLoginInfo>(serializedLoginInfo);

        // Assert: Verify that the deserialized instance matches the original.
        _ = deserializedLoginInfo.ShouldNotBeNull();
        deserializedLoginInfo.LoginProvider.ShouldBe(originalLoginInfo.LoginProvider);
        deserializedLoginInfo.ProviderKey.ShouldBe(originalLoginInfo.ProviderKey);
        deserializedLoginInfo.DisplayName.ShouldBe(originalLoginInfo.DisplayName);
    }

    /// <summary>
    /// Verifies that the properties of the <see cref="CustomUserLoginInfo"/> instance
    /// are correctly set during initialization.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_Properties_ShouldBeSetCorrectly()
    {
        // Arrange & Act: Create an instance of CustomUserLoginInfo with test data.
        CustomUserLoginInfo loginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Assert: Verify that the properties are set correctly.
        loginInfo.LoginProvider.ShouldBe("TestProvider");
        loginInfo.ProviderKey.ShouldBe("ProviderKey123");
        loginInfo.DisplayName.ShouldBe("DisplayName");
    }

    /// <summary>
    /// Tests that serialization and deserialization using System.Text.Json
    /// maintains the properties of the <see cref="CustomUserLoginInfo"/> instance.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_SystemTextJson_SerializeDeserialize_ShouldMaintainProperties()
    {
        // Arrange: Create an instance of CustomUserLoginInfo with test data.
        CustomUserLoginInfo originalLoginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Act: Serialize and then deserialize the instance using System.Text.Json.
        string serializedLoginInfo = JsonSerializer.Serialize(originalLoginInfo);
        CustomUserLoginInfo deserializedLoginInfo = JsonSerializer.Deserialize<CustomUserLoginInfo>(serializedLoginInfo);

        // Assert: Verify that the deserialized instance matches the original.
        _ = deserializedLoginInfo.ShouldNotBeNull();
        deserializedLoginInfo.LoginProvider.ShouldBe(originalLoginInfo.LoginProvider);
        deserializedLoginInfo.ProviderKey.ShouldBe(originalLoginInfo.ProviderKey);
        deserializedLoginInfo.DisplayName.ShouldBe(originalLoginInfo.DisplayName);
    }

    /// <summary>
    /// Verifies that the <see cref="CustomUserLoginInfo.UserLoginInfo"/> property
    /// returns a <see cref="UserLoginInfo"/> instance with the correct properties.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_UserLoginInfo_ShouldReturnCorrectInstance()
    {
        // Arrange: Create an instance of CustomUserLoginInfo with test data.
        CustomUserLoginInfo loginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Act: Retrieve the UserLoginInfo instance from the UserLoginInfo property.
        UserLoginInfo userLoginInfo = loginInfo.UserLoginInfo;

        // Assert: Verify that the properties of the UserLoginInfo instance match the original data.
        userLoginInfo.LoginProvider.ShouldBe("TestProvider");
        userLoginInfo.ProviderKey.ShouldBe("ProviderKey123");
        userLoginInfo.ProviderDisplayName.ShouldBe("DisplayName");
    }

    /// <summary>
    /// Verifies that the <see cref="CustomUserLoginInfo.UserLoginInfo"/> property
    /// is ignored during serialization.
    /// </summary>
    [Fact]
    public void CustomUserLoginInfo_UserLoginInfoProperty_IsIgnoredInSerialization()
    {
        // Arrange: Create an instance of CustomUserLoginInfo with test data.
        CustomUserLoginInfo originalLoginInfo = new("TestProvider", "ProviderKey123", "DisplayName");

        // Act - Using Newtonsoft.Json: Serialize the instance.
        string serializedLoginInfo = JsonConvert.SerializeObject(originalLoginInfo);

        // Assert: Verify that the serialized string does not contain the UserLoginInfo property.
        serializedLoginInfo.ShouldNotContain("UserLoginInfo");

        // Act - Using System.Text.Json: Serialize the instance.
        string systemTextJsonSerialized = JsonSerializer.Serialize(originalLoginInfo);

        // Assert: Verify that the serialized string does not contain the UserLoginInfo property.
        systemTextJsonSerialized.ShouldNotContain("UserLoginInfo");
    }
}