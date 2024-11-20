// ... existing copyright header ...

namespace Hexalith.DaprIdentityStore.Actors;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations and maintains associated indexes for user identities.
/// </summary>
public class UserIdentityActor : Actor, IUserIdentityActor 
{
    // Services for managing different aspects of user identity
    private readonly IUserIdentityCollectionService _collectionService;        // Manages the collection of all users
    private readonly IUserIdentityEmailCollectionService _emailCollectionService;  // Manages email-based user lookups
    private readonly IUserIdentityNameCollectionService _nameCollectionService;    // Manages username-based user lookups

    /// <summary>
    /// Cached state of the user actor to minimize state store access.
    /// This is lazily loaded when needed and updated on modifications.
    /// </summary>
    private UserActorState? _state;

    // ... existing constructor ...

    /// <summary>
    /// Adds claims to the user identity. Claims are unique statements about the user
    /// that can be used for authorization and user information.
    /// </summary>
    /// <param name="claims">Collection of claims to add to the user</param>
    /// <returns>Task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when user state is not found</exception>
    public async Task AddClaimsAsync(IEnumerable<Claim> claims)
    {
        // ... existing implementation ...
    }

    /// <summary>
    /// Creates a new user identity and establishes all necessary indexes.
    /// This includes setting up email and username lookups if provided.
    /// </summary>
    /// <param name="user">The user identity information to create</param>
    /// <returns>True if user was created, false if user already exists</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID doesn't match actor ID</exception>
    public async Task<bool> CreateAsync(UserIdentity user)
    {
        // ... existing implementation ...
    }

    // Add similar comprehensive comments for other public methods...

    /// <summary>
    /// Internal helper to retrieve and cache the actor's state.
    /// Minimizes state store access by maintaining a local cache.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The actor's state if it exists, null otherwise</returns>
    private async Task<UserActorState?> GetStateAsync(CancellationToken cancellationToken)
    {
        // ... existing implementation ...
    }
} 