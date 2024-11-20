// <copyright file="UserIdentityActorTest{Claims}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UnitTests.Actors;

using System.Security.Claims;

using Dapr.Actors.Runtime;

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
    /// Tests that AddClaimsAsync successfully adds new claims to an existing user
    /// and maintains uniqueness of claims.
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task AddClaimsAsyncShouldSucceed()
    {
        // Arrange
        // Create a test user identity with normalized username and email
        UserIdentity user = User;

        // Create initial state with existing claims
        UserActorState initialState = new()
        {
            User = user,
            Claims =
            [
                new() { UserId = user.Id, ClaimType = "existing", ClaimValue = "value" }
            ],
        };

        // Create claims to add
        List<Claim> newClaims =
        [
            new("role", "admin"),
            new("permission", "read"),
        ];

        // Create mock state manager
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup state retrieval to return existing user with claims
        stateManagerMoq
            .Setup(p => p.TryGetStateAsync<UserActorState>(
                DaprIdentityStoreConstants.UserIdentityStateName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConditionalValue<UserActorState>(true, initialState))
            .Verifiable();

        // Setup state update
        stateManagerMoq
            .Setup(p => p.SetStateAsync(
                DaprIdentityStoreConstants.UserIdentityStateName,
                It.Is<UserActorState>(state =>
                    state.Claims.Count() == 3 &&
                    state.Claims.Any(c => c.ClaimType == "existing") &&
                    state.Claims.Any(c => c.ClaimType == "role") &&
                    state.Claims.Any(c => c.ClaimType == "permission")),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        stateManagerMoq
            .Setup(p => p.SaveStateAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Create service mocks
        Mock<IUserIdentityCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

        // Setup claim service mock for each new claim
        foreach (Claim claim in newClaims)
        {
            claimServiceMoq
                .Setup(p => p.AddAsync(
                    It.Is<Claim>(c => c.Type == claim.Type && c.Value == claim.Value),
                    user.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

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
        await actor.AddClaimsAsync(newClaims);

        // Assert
        stateManagerMoq.Verify();
        claimServiceMoq.Verify();
    }

    /// <summary>
    /// Tests that GetClaimsAsync successfully retrieves all claims for a user.
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task GetClaimsAsyncShouldReturnAllUserClaims()
    {
        // Arrange
        UserIdentity user = User;

        // Create state with existing claims
        UserActorState state = new()
        {
            User = user,
            Claims =
            [
                new() { UserId = user.Id, ClaimType = "role", ClaimValue = "admin" },
                new() { UserId = user.Id, ClaimType = "permission", ClaimValue = "read" },
                new() { UserId = user.Id, ClaimType = "department", ClaimValue = "IT" }
            ],
        };

        // Create mock state manager
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup state retrieval
        stateManagerMoq
            .Setup(p => p.TryGetStateAsync<UserActorState>(
                DaprIdentityStoreConstants.UserIdentityStateName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConditionalValue<UserActorState>(true, state))
            .Verifiable();

        // Create service mocks
        Mock<IUserIdentityCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserIdentityLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

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
        IEnumerable<Claim> claims = await actor.GetClaimsAsync();

        // Assert
        stateManagerMoq.Verify();
        Assert.NotNull(claims);
        Assert.Equal(3, claims.Count());
        Assert.Contains(claims, c => c.Type == "role" && c.Value == "admin");
        Assert.Contains(claims, c => c.Type == "permission" && c.Value == "read");
        Assert.Contains(claims, c => c.Type == "department" && c.Value == "IT");
    }
}