# Hexalith.DaprIdentityStore Core Library

This folder contains the core implementation of the Hexalith.DaprIdentityStore library.

For complete documentation, installation instructions, and usage examples, please refer to the [main project README](../../README.md).

## Project Structure

- **Actors/**: Contains the Dapr actor implementations for identity management
- **States/**: Contains actor state definitions and models
- **Interfaces/**: Contains service and repository interfaces
- **Services/**: Contains concrete implementations of identity services

## Development

This library is part of the Hexalith.DaprIdentityStore solution. For development guidelines and contribution information, please see the main project documentation.

## Quick Links

- [Main Documentation](../../README.md)
- [NuGet Package](https://www.nuget.org/packages/Hexalith.DaprIdentityStore.Server)
- [Issue Tracker](https://github.com/Hexalith/Hexalith.DaprIdentityStore/issues)

# External Identity Provider Roles Synchronization

This feature allows synchronizing user roles from external identity providers to the local identity store when users log in using external authentication providers.

## How It Works

When a user logs in or links an account with an external identity provider (such as Google, Microsoft, Okta, etc.), the system:

1. Authenticates the user through the external provider
2. Extracts role claims from the external provider's user principal
3. Synchronizes these roles with the local identity store

The synchronization happens in the following scenarios:
- When an existing user logs in via an external provider
- When a new user creates an account via an external provider
- When a user links a new external provider to their existing account

## Supported Role Claim Types

The system looks for role claims with the following claim types:
- `ClaimTypes.Role` (http://schemas.microsoft.com/ws/2008/06/identity/claims/role)
- `role` (common in OAuth providers)
- `roles` (used by some identity providers)
- Any claim type ending with `/roles` (common in JWT formats)

## Implementation

The implementation is provided by the `ExternalLoginExtensions.cs` class, which extends `UserManager<CustomUser>` with the `SyncExternalProviderRolesAsync` method.

```csharp
public static async Task SyncExternalProviderRolesAsync(
    this UserManager<CustomUser> userManager,
    CustomUser user,
    ExternalLoginInfo externalLoginInfo)
```

## Configuring External Identity Providers

When configuring external identity providers, ensure they are set up to include role claims in the authentication token. The specific configuration varies by provider, but generally involves:

1. Registering your application with the external provider
2. Configuring the proper scopes and claims mapping
3. Setting up roles/groups in the external provider
4. Mapping those roles to appropriate claims in the token

## Notes

- Roles are synchronized during login, so changes in external provider roles will be reflected on the next login
- The system only adds roles from the external provider; it does not remove local roles
- If a role doesn't exist in the local system but is provided by the external provider, it will be added to the user
