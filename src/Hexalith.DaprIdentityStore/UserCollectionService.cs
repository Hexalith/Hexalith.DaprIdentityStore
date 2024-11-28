namespace Hexalith.DaprIdentityStore;

public class UserCollectionService : IUserCollectionService
{
    private readonly IUserStore<CustomUser> _userStore;

    public UserCollectionService(IUserStore<CustomUser> userStore)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    // ...existing code...

    public async Task<IEnumerable<CustomUser>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var ids = await AllAsync();
        var users = new List<CustomUser>();
        foreach (var id in ids)
        {
            var user = await _userStore.FindByIdAsync(id, cancellationToken);
            if (user is not null)
            {
                users.Add(user);
            }
        }
        return users;
    }
}
