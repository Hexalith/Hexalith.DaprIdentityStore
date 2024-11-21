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
    /// Gets the collection identifier for accessing all roles in the store.
    /// This constant is used as the actor ID when querying or managing the complete set of roles.
    /// </summary>
    /// <value>
    /// The string "AllRoles", which serves as a unique identifier for the collection of all role records.
    /// </value>
    /// <remarks>
    /// This identifier is used in operations that need to access or modify the complete set of roles,
    /// such as role enumeration or batch operations.
    /// </remarks>
    public static string AllRolesCollectionActorId => "AllRoles";

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
    /// Gets the default actor type name for role actors.
    /// This constant defines the base actor type used for role management.
    /// </summary>
    /// <value>
    /// The string "Role", which identifies the actor type responsible for managing roles.
    /// </value>
    public static string DefaultRoleActorTypeName => "Role";

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
    public static string DefaultUserActorTypeName => "User";

    /// <summary>
    /// Gets the index type name for role claim lookups.
    /// This constant defines the actor type used for claim-based role lookups.
    /// </summary>
    /// <value>
    /// The string "RoleClaimIndex", which identifies the actor type responsible for claim-based role lookups.
    /// </value>
    public static string RoleClaimIndexActorTypeName => "RoleClaimIndex";

    /// <summary>
    /// Gets the collection type name for storing roles.
    /// This constant defines the actor type used for managing collections of roles.
    /// </summary>
    /// <value>
    /// The string "Roles", which identifies the actor type responsible for managing collections of roles.
    /// </value>
    public static string RoleCollectionActorTypeName => "Roles";

    /// <summary>
    /// Gets the index type name for role name lookups.
    /// This constant defines the actor type used for name-based role lookups.
    /// </summary>
    /// <value>
    /// The string "RoleNameIndex", which identifies the actor type responsible for name-based role lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient role lookups by name, which is essential
    /// for authorization and role management operations.
    /// </remarks>
    public static string RoleNameIndexActorTypeName => "RoleNameIndex";

    /// <summary>
    /// Gets the state name for role.
    /// This constant defines the state name used for storing role information.
    /// </summary>
    /// <value>
    /// The string "State", which identifies the state name for role.
    /// </value>
    /// <remarks>
    /// This state name is used in operations that need to access or modify the state of roles,
    /// such as role management and role validation.
    /// </remarks>
    public static string RoleStateName => "State";

    /// <summary>
    /// Gets the index type name for user claim lookups.
    /// This constant defines the actor type used for claim-based user lookups.
    /// </summary>
    /// <value>
    /// The string "UserClaimIndex", which identifies the actor type responsible for claim-based user lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient user lookups by claims, which is essential
    /// for authorization and user management operations.
    /// </remarks>
    public static string UserClaimIndexActorTypeName => "UserClaimIndex";

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
    public static string UserCollectionActorTypeName => "Users";

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
    /// Gets the index type name for user login lookups.
    /// This constant defines the actor type used for login-based user lookups.
    /// </summary>
    /// <value>
    /// The string "UserLoginIndex", which identifies the actor type responsible for login-based user lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient user lookups by login information, which is essential
    /// for authentication and user management operations.
    /// </remarks>
    public static string UserLoginIndexActorTypeName => "UserLoginIndex";

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

    /// <summary>
    /// Gets the state name for user identity.
    /// This constant defines the state name used for storing user identity information.
    /// </summary>
    /// <value>
    /// The string "State", which identifies the state name for user identity.
    /// </value>
    /// <remarks>
    /// This state name is used in operations that need to access or modify the state of user identities,
    /// such as authentication, profile management, and credential validation.
    /// </remarks>
    public static string UserStateName => "State";

    /// <summary>
    /// Gets the index type name for user token lookups.
    /// This constant defines the actor type used for token-based user lookups.
    /// </summary>
    /// <value>
    /// The string "UserTokenIndex", which identifies the actor type responsible for token-based user lookups.
    /// </value>
    /// <remarks>
    /// This index type enables efficient user lookups by tokens, which is essential
    /// for authentication and user management operations.
    /// </remarks>
    public static string UserTokenIndexActorTypeName => "UserTokenIndex";
}