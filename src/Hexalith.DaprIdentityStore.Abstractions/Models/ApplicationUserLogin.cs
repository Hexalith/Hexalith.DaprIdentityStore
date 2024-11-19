namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user's login information in the Dapr identity store.
/// Extends IdentityUserLogin with string-based user identifiers to store external login provider data.
/// </summary>
public class ApplicationUserLogin : IdentityUserLogin<string>
{
}