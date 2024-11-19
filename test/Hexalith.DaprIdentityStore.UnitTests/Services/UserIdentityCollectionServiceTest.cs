// <copyright file="UserIdentityCollectionServiceTest.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Services;

using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Moq;

/// <summary>
/// Unit tests for the UserIdentityCollectionService class which manages collections of user identities.
/// This service is backed by a Dapr actor for distributed state management.
/// </summary>
public class UserIdentityCollectionServiceTest
{
    /// <summary>
    /// Tests that adding a new user ID to the collection succeeds and can be retrieved.
    /// This test verifies both the AddUserAsync and AllAsync methods of the service.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task AddNewUserToTheCollectionShouldSucceed()
    {
        // Arrange
        Mock<IKeyHashActor> actorMoq = new();
        Mock<IActorProxyFactory> proxyFactoryMoq = new();
        _ = proxyFactoryMoq.Setup(p => p.CreateActorProxy<IKeyHashActor>(
            It.IsAny<ActorId>(),
            It.IsAny<string>(),
            It.IsAny<ActorProxyOptions>()))
        .Returns(actorMoq.Object);

        // Create a test actor host for the KeyHashActor with the UserCollection actor type
        ActorHost host = ActorHost.CreateForTest<KeyHashActor>(
            DaprIdentityStoreConstants.UserCollectionActorTypeName,
            new ActorTestOptions
            {
                ProxyFactory = proxyFactoryMoq.Object,
            });

        // Initialize the service with the test actor host
        UserIdentityCollectionService service = new(host);

        // Generate a random GUID to use as the test user ID
        string id = Guid.NewGuid().ToString();

        // Act
        // Attempt to add the new user ID to the collection
        await service.AddUserAsync(id);

        // Assert
        // Retrieve all users and verify the new ID is present in the collection
        IEnumerable<string> all = await service.AllAsync();
        Assert.Contains(id, all);
    }
}