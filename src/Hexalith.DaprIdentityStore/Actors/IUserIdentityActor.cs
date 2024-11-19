namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors;

using Hexalith.DaprIdentityStore.Models;

public interface IUserIdentityActor : IActor
{
    Task<bool> CreateAsync(UserIdentity user);

    Task DeleteAsync(UserIdentity user);

    Task<bool> ExistsAsync();

    Task<UserIdentity?> FindAsync();

    Task<UserIdentity?> FindByEmailAsync();

    Task<UserIdentity?> FindByNameAsync();

    Task UpdateAsync(UserIdentity user);
}