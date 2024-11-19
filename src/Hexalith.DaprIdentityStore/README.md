# Hexalith.DaprIdentityStore

## Overview
Hexalith.DaprIdentityStore is a custom identity store implementation using Dapr (Distributed Application Runtime) for ASP.NET Core Identity. It leverages Dapr's actor model and state management capabilities to provide a scalable and distributed identity solution.

## Features
- Distributed user management using Dapr Actors
- Scalable identity storage with Dapr state management
- Compatible with ASP.NET Core Identity
- Support for claims and role-based authentication
- Built-in concurrency handling
- Cloud-agnostic implementation

## Architecture

### Components
1. **User Actor**
   - Manages individual user state and operations
   - Handles user claims and roles
   - Provides atomic operations for user data updates

2. **User Actor State**
   - Stores user information including:
     - Basic profile data
     - Security stamps
     - Login information
     - Claims and roles

### Integration with ASP.NET Core Identity
The store implements standard Identity interfaces:
- `IUserStore<TUser>`
- `IUserLoginStore<TUser>`
- `IUserClaimStore<TUser>`
- `IUserPasswordStore<TUser>`
- `IUserSecurityStampStore<TUser>`
- `IUserEmailStore<TUser>`
- `IUserPhoneNumberStore<TUser>`
- `IUserTwoFactorStore<TUser>`
- `IUserLockoutStore<TUser>`
- `IQueryableUserStore<TUser>`

## Setup and Configuration

### Prerequisites
- .NET 9.0 or later
- Dapr runtime installed
- A compatible Dapr state store component (e.g., Redis, Azure CosmosDB)

### Installation

Add the package reference to your project:
