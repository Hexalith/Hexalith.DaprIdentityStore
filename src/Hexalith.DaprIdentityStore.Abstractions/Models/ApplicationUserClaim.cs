namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a claim that belongs to a user in the Dapr identity store.
/// Extends IdentityUserClaim with string-based user identifiers.
/// </summary>
public class ApplicationUserClaim : IdentityUserClaim<string>
{
}