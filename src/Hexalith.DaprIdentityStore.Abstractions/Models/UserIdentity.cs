namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user identity in the Dapr identity store.
/// Extends the base IdentityUser class to provide core user identity functionality.
/// This class serves as the primary user entity for authentication and user management.
/// </summary>
public class UserIdentity : IdentityUser
{
}