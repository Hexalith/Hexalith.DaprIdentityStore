namespace Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an application role in the Dapr identity store.
/// Extends the base IdentityRole class to provide role-based authorization capabilities.
/// </summary>
public class ApplicationRole : IdentityRole
{
}