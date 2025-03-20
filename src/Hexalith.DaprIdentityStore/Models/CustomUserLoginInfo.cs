namespace Hexalith.DaprIdentityStore.Models;

using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents login information and source for a user record.
/// </summary>
[DataContract]
public record CustomUserLoginInfo(
    [property: DataMember(Order = 1)] string LoginProvider,
    [property: DataMember(Order = 2)] string ProviderKey,
    [property: DataMember(Order = 3)] string? DisplayName)
{
    /// <summary>
    /// Creates a new instance of <see cref="CustomUserLoginInfo"/>
    /// </summary>
    /// <param name="info">The <see cref="UserLoginInfo"/> to copy.</param>
    public CustomUserLoginInfo(UserLoginInfo info)
        : this(
              (info ?? throw new ArgumentNullException(nameof(info))).LoginProvider,
              info.ProviderKey,
              info.ProviderDisplayName)
    {
    }
}