namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.States;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

public class UserIdentityActor : Actor, IUserIdentityActor
{
    private const string StateKey = "State";
    private UserActorState? _state;

    public UserIdentityActor(ActorHost host) : base(host)
    {
    }

    public static string ActorCollectionTypeName => "UserIdentities";
    public static string AllCollectionId => "All";
    public static string DefaultActorTypeName => "UserIdentity";

    public async Task<bool> CreateAsync(ApplicationUserIdentity user)
    {
        if (user.Id != Id.ToUnescapeString())
        {
            throw new InvalidOperationException($"{Host.ActorTypeInfo.ActorTypeName} Id '{Id.ToUnescapeString()}' does not match user Id '{user.Id}'.");
        }
        if (_state != null)
        {
            return false;
        }
        _state = new UserActorState { User = user };

        await StateManager.AddStateAsync(StateKey, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);

        IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        _ = await collection.AddAsync(user.Id);

        return true;
    }

    public async Task DeleteAsync(ApplicationUserIdentity user)
    {
        if (await GetStateAsync(CancellationToken.None) != null)
        {
            _state = null;
            await StateManager.RemoveStateAsync(StateKey, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
            IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
            await collection.RemoveAsync(user.Id);
        }
    }

    public async Task<bool> ExistsAsync() => await GetStateAsync(CancellationToken.None) != null;

    public async Task<ApplicationUserIdentity?> FindAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state?.User;
    }

    private async Task<UserActorState?> GetStateAsync(CancellationToken cancellationToken)
    {
        if (_state is null)
        {
            ConditionalValue<UserActorState> result = await StateManager.TryGetStateAsync<UserActorState>(StateKey, cancellationToken);
            if (result.HasValue)
            {
                _state = result.Value;
            }
        }
        return _state;
    }
}