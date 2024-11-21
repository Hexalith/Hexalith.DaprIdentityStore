// <copyright file="UserIdentityActorTest{User}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Actors;

using Dapr.Actors.Runtime;

using FluentAssertions;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Moq;

/// <summary>
/// Unit tests for the UserIdentityActor class which handles user identity operations.
/// Tests cover core functionality including user creation, deletion, and state management.
/// </summary>
public partial class UserIdentityActorTest
{
    /// <summary>
    /// Gets a sample user identity for testing purposes.
    /// Contains normalized username and email for index testing.
    /// </summary>
    private CustomUser User => new()
    {
        Id = "user 1",
        UserName = "user one",
        NormalizedUserName = "USERONE",
        NormalizedEmail = "USER1@HEXALITH.COM",
    };

    /// <summary>
    /// Tests that AddUserAsync successfully adds a new user to the actor state
    /// and creates all necessary indexes (user collection, email, and username).
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task AddUserAsyncShouldSucceed()
    {
        // Arrange
        // Create a test user identity with normalized username and email
        CustomUser user = User;

        // Create a mock state manager to verify actor state operations
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup the mock to verify that AddStateAsync is called exactly once
        // with the correct state name and user data
        stateManagerMoq.Setup(p => p.AddStateAsync<UserActorState>(
            DaprIdentityStoreConstants.UserIdentityStateName,
            It.Is<UserActorState>(p => p.User.Id == user.Id),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        stateManagerMoq.Setup(p => p.SaveStateAsync(
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Create services for the actor to use
        Mock<IUserCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

        collectionServiceMoq.Setup(p => p.AddAsync(user.Id))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);
        emailServiceMoq.Setup(p => p.AddAsync(user.NormalizedEmail, user.Id))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);
        nameServiceMoq.Setup(p => p.AddAsync(user.NormalizedUserName, user.Id))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Create a test actor host with the specified user ID
        ActorHost actorHost = ActorHost.CreateForTest<UserIdentityActor>(
            new ActorTestOptions { ActorId = user.Id.ToActorId() });

        // Initialize the actor with the mock state manager
        UserIdentityActor actor = new(
            actorHost,
            collectionServiceMoq.Object,
            emailServiceMoq.Object,
            nameServiceMoq.Object,
            claimServiceMoq.Object,
            tokenServiceMoq.Object,
            loginServiceMoq.Object,
            stateManagerMoq.Object);

        // Act
        // Attempt to create the user using the actor
        bool created = await actor.CreateAsync(user);

        // Assert
        // Verify that the state manager was called as expected
        _ = created.Should().BeTrue();
        stateManagerMoq.Verify();
        collectionServiceMoq.Verify();
        emailServiceMoq.Verify();
        nameServiceMoq.Verify();
    }

    /// <summary>
    /// Tests that DeleteAsync successfully removes a user and cleans up all associated
    /// indexes (user collection, email, and username).
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task DeleteAsyncShouldSucceed()
    {
        // Arrange
        // Arrange
        // Create a test user identity with normalized username and email
        CustomUser user = User;

        // Create mock state manager
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup state retrieval to return existing user
        stateManagerMoq
            .Setup(p => p.TryGetStateAsync<UserActorState>(
                DaprIdentityStoreConstants.UserIdentityStateName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConditionalValue<UserActorState>(true, new UserActorState { User = user }))
            .Verifiable();

        // Setup state removal operations
        stateManagerMoq
            .Setup(p => p.RemoveStateAsync(
                DaprIdentityStoreConstants.UserIdentityStateName,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        stateManagerMoq
            .Setup(p => p.SaveStateAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Create service mocks
        Mock<IUserCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

        // Setup service operations
        collectionServiceMoq
            .Setup(p => p.RemoveAsync(user.Id))
            .Returns(Task.CompletedTask)
            .Verifiable();

        emailServiceMoq
            .Setup(p => p.RemoveAsync(user.NormalizedEmail))
            .Returns(Task.CompletedTask)
            .Verifiable();

        nameServiceMoq
            .Setup(p => p.RemoveAsync(user.NormalizedUserName))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Create actor host and actor
        ActorHost actorHost = ActorHost.CreateForTest<UserIdentityActor>(
            new ActorTestOptions { ActorId = user.Id.ToActorId() });

        UserIdentityActor actor = new(
            actorHost,
            collectionServiceMoq.Object,
            emailServiceMoq.Object,
            nameServiceMoq.Object,
            claimServiceMoq.Object,
            tokenServiceMoq.Object,
            loginServiceMoq.Object,
            stateManagerMoq.Object);

        // Act
        await actor.DeleteAsync();

        // Assert
        stateManagerMoq.Verify();
        collectionServiceMoq.Verify();
        emailServiceMoq.Verify();
        nameServiceMoq.Verify();
    }
}