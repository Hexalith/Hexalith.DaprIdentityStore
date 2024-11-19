namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors;

public interface IUserIdentityActor : IActor
{
    Task<bool> CreateAsync(ApplicationUserIdentity user);

    Task DeleteAsync(ApplicationUserIdentity user);

    Task<bool> ExistsAsync();
    Task<ApplicationUserIdentity?> FindAsync();
}