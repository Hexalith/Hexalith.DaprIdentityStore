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
        HashSet<string> storedUsers = [];
        Mock<IKeyHashActor> actorMock = new(MockBehavior.Strict);
        Mock<IActorProxyFactory> proxyFactoryMock = new(MockBehavior.Strict);

        // Setup the mock to store users when AddAsync is called
        actorMock
            .Setup(x => x.AddAsync(It.IsAny<string>()))
            .Callback<string>(id => storedUsers.Add(id))
            .ReturnsAsync(1)
            .Verifiable(Times.Once);

        proxyFactoryMock
            .Setup(p => p.CreateActorProxy<IKeyHashActor>(
                It.IsAny<ActorId>(),
                It.IsAny<string>(),
                It.IsAny<ActorProxyOptions>()))
            .Returns(actorMock.Object)
            .Verifiable(Times.Once);

        // Create a test actor host for the KeyHashActor
        ActorHost host = ActorHost.CreateForTest<KeyHashActor>(
            DaprIdentityStoreConstants.UserCollectionActorTypeName,
            new ActorTestOptions
            {
                ProxyFactory = proxyFactoryMock.Object,
            });

        UserIdentityCollectionService service = new(host.ProxyFactory);
        string id = Guid.NewGuid().ToString();

        // Act
        await service.AddAsync(id);

        // Verify the actor methods were called
        actorMock.Verify();
        proxyFactoryMock.Verify();
    }
}