# Hexalith.DaprIdentityStore.Abstractions

## Overview

Hexalith.DaprIdentityStore.Abstractions is a core library that provides abstractions and models for implementing distributed identity management using Dapr Actors. This library enables scalable and reliable user identity management in distributed applications.

## Architecture

The library is structured into several key components:

### Actor System

#### IUserIdentityActor

The core actor interface that handles user identity operations:

- User creation and management
- Password management
- Claims and roles handling
- Authentication token management

#### Actor States

- `UserActorState`: Maintains the actor's internal state including:
  - User profile information
  - Security stamps
  - Authentication tokens
  - Claims and roles

### Models

#### Core Identity Models

- `UserIdentity`: The primary user identity model containing:
  - Basic user information (Id, Username, Email)
  - Security information (PasswordHash, SecurityStamp)
  - Two-factor authentication settings
  - Lockout settings

#### Authentication Models

- `ApplicationRole`: Represents user roles in the system
- `ApplicationUserClaim`: Represents user claims
- `ApplicationUserLogin`: Manages external login providers
- `ApplicationUserToken`: Handles authentication tokens

### Helpers

#### DaprIdentityStoreActorProxyHelper

Provides utility methods for:

- Actor proxy creation and management
- Actor communication helpers
- State management utilities

### Error Handling

#### HexalithIdentityErrorDescriber

Provides standardized error descriptions for common identity operations:

- Authentication failures
- Password validation
- User validation
- Role and claim operations

## Usage

### Basic Setup
