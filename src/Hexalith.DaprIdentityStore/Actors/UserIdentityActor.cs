namespace Hexalith.DaprIdentityStore.Actors;

using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
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

    public async Task<bool> CreateAsync(UserIdentity user)
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

        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(user.NormalizedEmail);
            await emailIndex.SetAsync(user.Id);
        }
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(user.NormalizedUserName);
            await nameIndex.SetAsync(user.Id);
        }

        return true;
    }

    public async Task DeleteAsync(UserIdentity user)
    {
        if (await GetStateAsync(CancellationToken.None) != null)
        {
            _state = null;
            await StateManager.RemoveStateAsync(StateKey, CancellationToken.None);
            await StateManager.SaveStateAsync(CancellationToken.None);
            IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
            await collection.RemoveAsync(user.Id);
            if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
            {
                IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(user.NormalizedEmail);
                await emailIndex.RemoveAsync();
            }
            if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
            {
                IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(user.NormalizedUserName);
                await nameIndex.RemoveAsync();
            }
        }
    }

    public async Task<bool> ExistsAsync() => await GetStateAsync(CancellationToken.None) != null;

    public async Task<UserIdentity?> FindAsync()
    {
        _state = await GetStateAsync(CancellationToken.None);
        return _state?.User;
    }

    public Task<UserIdentity?> FindByEmailAsync() => throw new NotImplementedException();

    public Task<UserIdentity?> FindByNameAsync() => throw new NotImplementedException();

    public async Task UpdateAsync(UserIdentity user)
    {
        _state = await GetStateAsync(CancellationToken.None);
        if (_state is null)
        {
            throw new InvalidOperationException($"User '{user.Id}' not found.");
        }

        _ = _state.User;
        _state.User = user;
        await StateManager.SetStateAsync(StateKey, _state, CancellationToken.None);
        await StateManager.SaveStateAsync(CancellationToken.None);
        IKeyHashActor collection = ActorProxy.DefaultProxyFactory.CreateAllUsersProxy();
        await collection.RemoveAsync(user.Id);
        if (!string.IsNullOrWhiteSpace(user.NormalizedEmail))
        {
            IKeyValueActor emailIndex = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(user.NormalizedEmail);
            await emailIndex.RemoveAsync();
        }
        if (!string.IsNullOrWhiteSpace(user.NormalizedUserName))
        {
            IKeyValueActor nameIndex = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(user.NormalizedUserName);
            await nameIndex.RemoveAsync();
        }
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