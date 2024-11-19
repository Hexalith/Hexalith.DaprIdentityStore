namespace Hexalith.DaprIdentityStore.Errors;

using Microsoft.AspNetCore.Identity;

public class HexalithIdentityErrorDescriber : IdentityErrorDescriber
{
    public IdentityError DuplicateUserId(string userId) => new()
    {
        Code = nameof(DuplicateUserId),
        Description = $"User id '{userId}' is already taken."
    };
}