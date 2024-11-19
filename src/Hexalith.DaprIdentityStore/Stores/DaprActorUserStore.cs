namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Errors;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Microsoft.AspNetCore.Identity;

public class DaprActorUserStore
    : UserStoreBase<UserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public DaprActorUserStore(
            IActorProxyFactory actorProxyFactory)
        : base(new HexalithIdentityErrorDescriber()) => _actorProxyFactory = actorProxyFactory;

    public override IQueryable<UserIdentity> Users => GetUsersAsync().GetAwaiter().GetResult().AsQueryable();

    public override Task AddClaimsAsync(UserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task AddLoginAsync(UserIdentity user, UserLoginInfo login, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override async Task<IdentityResult> CreateAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = GetUserActor(user.Id);
        bool created = await actor.CreateAsync(user);
        if (created)
        {
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.NormalizedUserName));
    }

    public override async Task<IdentityResult> DeleteAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = GetUserActor(user.Id);
        await actor.DeleteAsync(user);
        return IdentityResult.Success;
    }

    public override async Task<UserIdentity?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserEmailIndexProxy(normalizedEmail);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    public override async Task<UserIdentity?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = GetUserActor(userId);
        return await actor.FindAsync();
    }

    public override async Task<UserIdentity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUserName);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateUserNameIndexProxy(normalizedUserName);
        string? userId = await actor.GetAsync();
        return string.IsNullOrWhiteSpace(userId) ? null : await FindByIdAsync(userId, cancellationToken);
    }

    public override Task<IList<Claim>> GetClaimsAsync(UserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(UserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IUserIdentityActor GetUserActor(string id)
        => _actorProxyFactory.CreateActorProxy<IUserIdentityActor>(id.ToActorId(), UserIdentityActor.DefaultActorTypeName);

    public override Task<IList<UserIdentity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task RemoveClaimsAsync(UserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task RemoveLoginAsync(UserIdentity user, string loginProvider, string providerKey, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task ReplaceClaimAsync(UserIdentity user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override async Task<IdentityResult> UpdateAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (await FindByIdAsync(user.Id, cancellationToken) == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = $"A user with the Id '{user.Id}' could not be found." });
        }
        IUserIdentityActor actor = GetUserActor(user.Id);
        await actor.UpdateAsync(user);
        return IdentityResult.Success;
    }

    protected override Task AddUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    protected override Task<ApplicationUserToken?> FindTokenAsync(UserIdentity user, string loginProvider, string name, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<UserIdentity?> FindUserAsync(string userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task RemoveUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    private async Task<List<UserIdentity>> GetUsersAsync()
    {
        ThrowIfDisposed();

        List<UserIdentity> users = [];
        IKeyHashActor allProxy = _actorProxyFactory.CreateActorProxy<IKeyHashActor>(UserIdentityActor.AllUsersCollectionId.ToActorId(), UserIdentityActor.ActorCollectionTypeName);
        IEnumerable<string> userIds = await allProxy.AllAsync();
        List<Task<UserIdentity?>> tasks = [];
        foreach (string userId in userIds)
        {
            IUserIdentityActor userProxy = GetUserActor(userId);
            tasks.Add(userProxy.FindAsync());
        }
        return (await Task.WhenAll(tasks)).Where(p => p != null).OfType<UserIdentity>().ToList();
    }
}