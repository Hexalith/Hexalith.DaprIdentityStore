namespace Hexalith.DaprIdentityStore;

public interface IGetAllUsers
{
    Task<IEnumerable<CustomUser>> GetAllUsersAsync(CancellationToken cancellationToken);
}
