// <copyright file="UserIdentityActorTest.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Actors;

using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Moq;

/// <summary>
/// Unit tests for the UserIdentityActor class which handles user identity operations.
/// </summary>
public class UserIdentityActorTest
{
    /// <summary>
    /// Tests that AddUserAsync correctly adds a new user to the actor state.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task AddUserAsync_Should_Add_User()
    {
        // Arrange
        // Create a test user identity with normalized username and email
        UserIdentity user = new()
        {
            Id = "user1",
            UserName = "user one",
            NormalizedUserName = "USERONE",
            NormalizedEmail = "USER1@HEXALITH.COM",
        };

        // Create a mock state manager to verify actor state operations
        Mock<IActorStateManager> stateManagerMoq = new();

        // Setup the mock to verify that AddStateAsync is called exactly once
        // with the correct state name and user data
        stateManagerMoq.Setup(p => p.AddStateAsync<UserActorState>(
            DaprIdentityStoreConstants.UserIdentityStateName,
            It.Is<UserActorState>(p => p.User.Id == user.Id),
            It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);

        // Create a test actor host with the specified user ID
        ActorHost actorHost = ActorHost.CreateForTest<UserIdentityActor>(
            new ActorTestOptions { ActorId = user.Id.ToActorId() });

        // Initialize the actor with the mock state manager
        IUserIdentityActor actor = new UserIdentityActor(actorHost, stateManagerMoq.Object);

        // Act
        // Attempt to create the user using the actor
        _ = await actor.CreateAsync(user);

        // Assert
        // Verify that the state manager was called as expected
        stateManagerMoq.Verify();
    }
}