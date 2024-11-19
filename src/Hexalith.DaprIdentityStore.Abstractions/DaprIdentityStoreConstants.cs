// <copyright file="DaprIdentityStoreConstants.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore;

/// <summary>
/// Contains constant values used by the Dapr Identity Store implementation.
/// These constants define the actor and collection names used for identity management.
/// </summary>
/// <remarks>
/// This class provides centralized access to string constants used throughout the Dapr Identity Store
/// for consistent naming of actors, collections, and indexes. These values are crucial for the proper
/// functioning of the distributed identity store implementation.
/// </remarks>
public static class DaprIdentityStoreConstants
{
    /// <summary>
    /// Gets the collection identifier for accessing all users in the store.
    /// This constant is used as the actor ID when querying or managing the complete set of users.
    /// </summary>
    /// <value>
    /// The string "AllUsers", which serves as a unique identifier for the collection of all user records.
    /// </value>
    /// <remarks>
    /// This identifier is used in operations that need to access or modify the complete set of users,
    /// such as user enumeration or batch operations.
    /// </remarks>
    public static string AllUsersCollectionActorId => "AllUsers";

    /// <summary>
    /// Gets the default actor type name for user identity actors.
    /// This constant defines the base actor type used for individual user identity management.
    /// </summary>
    /// <value>
    /// The string "UserIdentity", which identifies the actor type responsible for managing individual user identities.
    /// </value>
    /// <remarks>
    /// This actor type handles operations specific to individual user identities, such as
    /// authentication, profile management, and credential validation.
    /// </remarks>
    public static string DefaultUserActorTypeName => "UserIdentity";

    /// <summary>
    /// Gets the collection type name for storing user identities.
    /// This constant defines the actor type used for managing collections of user identities.
    /// </summary>
    /// <value>
    /// The string "UserIdentities", which identifies the actor type responsible for managing collections of user identities.
    /// </value>
    /// <remarks>
    /// This collection type is used for operations that involve multiple user identities,
    /// such as querying users or performing batch operations on user groups.
    /// </remarks>
    public static string UserCollectionActorTypeName => "UserIdentities";

    /// <summary>
    /// Gets the index type name for user email lookups.
    /// This constant defines the actor type used for email-based user lookups.
    /// </summary>
    /// <value>
    /// The string "UserEmailIndex", which identifies the actor type responsible for email-based user lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient user lookups by email address, which is essential
    /// for authentication and user management operations.
    /// </remarks>
    public static string UserEmailIndexActorTypeName => "UserEmailIndex";

    /// <summary>
    /// Gets the index type name for username lookups.
    /// This constant defines the actor type used for username-based user lookups.
    /// </summary>
    /// <value>
    /// The string "UserNameIndex", which identifies the actor type responsible for username-based user lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient user lookups by username, which is essential
    /// for authentication and user management operations.
    /// </remarks>
    public static string UserNameIndexActorTypeName => "UserNameIndex";
}