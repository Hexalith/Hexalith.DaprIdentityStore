namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an authentication token for a user in the Dapr identity store.
/// Extends IdentityUserToken with string-based user identifiers to store authentication tokens.
/// </summary>
public class ApplicationUserToken : IdentityUserToken<string>
{
}