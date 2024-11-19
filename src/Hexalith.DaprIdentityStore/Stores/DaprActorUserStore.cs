namespace Hexalith.DaprIdentityStore.Stores;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.DaprIdentityStore.Actors;
using Hexalith.DaprIdentityStore.Errors;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.Infrastructure.DaprRuntime.Actors;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Microsoft.AspNetCore.Identity;

public class DaprActorUserStore
    : UserStoreBase<ApplicationUserIdentity, string, ApplicationUserClaim, ApplicationUserLogin, ApplicationUserToken>
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public DaprActorUserStore(
            IActorProxyFactory actorProxyFactory)
        : base(new HexalithIdentityErrorDescriber()) => _actorProxyFactory = actorProxyFactory;

    public override IQueryable<ApplicationUserIdentity> Users => GetUsersAsync().GetAwaiter().GetResult().AsQueryable();

    public override Task AddClaimsAsync(ApplicationUserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task AddLoginAsync(ApplicationUserIdentity user, UserLoginInfo login, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override async Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken = default)
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
        return IdentityResult.Failed(new IdentityError { Code = "DuplicateUser", Description = $"A user with the same Id '{user.Id}' already exists." });
    }

    public override async Task<IdentityResult> DeleteAsync(ApplicationUserIdentity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IUserIdentityActor actor = GetUserActor(user.Id);
        await actor.DeleteAsync(user);
        return IdentityResult.Success;
    }

    public override Task<ApplicationUserIdentity?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<ApplicationUserIdentity?> FindByIdAsync(string userId, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<ApplicationUserIdentity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<IList<Claim>> GetClaimsAsync(ApplicationUserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IUserIdentityActor GetUserActor(string id)
        => _actorProxyFactory.CreateActorProxy<IUserIdentityActor>(id.ToActorId(), UserIdentityActor.DefaultActorTypeName);

    public override Task<IList<ApplicationUserIdentity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task RemoveClaimsAsync(ApplicationUserIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task RemoveLoginAsync(ApplicationUserIdentity user, string loginProvider, string providerKey, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task ReplaceClaimAsync(ApplicationUserIdentity user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<IdentityResult> UpdateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    protected override Task AddUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    protected override Task<ApplicationUserToken?> FindTokenAsync(ApplicationUserIdentity user, string loginProvider, string name, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<ApplicationUserIdentity?> FindUserAsync(string userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task<ApplicationUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override Task RemoveUserTokenAsync(ApplicationUserToken token) => throw new NotImplementedException();

    private async Task<List<ApplicationUserIdentity>> GetUsersAsync()
    {
        ThrowIfDisposed();

        List<ApplicationUserIdentity> users = [];
        IKeyHashActor allProxy = _actorProxyFactory.CreateActorProxy<IKeyHashActor>(UserIdentityActor.AllCollectionId.ToActorId(), UserIdentityActor.ActorCollectionTypeName);
        IEnumerable<string> userIds = await allProxy.AllAsync();
        List<Task<ApplicationUserIdentity?>> tasks = [];
        foreach (string userId in userIds)
        {
            IUserIdentityActor userProxy = GetUserActor(userId);
            tasks.Add(userProxy.FindAsync());
        }
        return (await Task.WhenAll(tasks)).Where(p => p != null).OfType<ApplicationUserIdentity>().ToList();
    }
}