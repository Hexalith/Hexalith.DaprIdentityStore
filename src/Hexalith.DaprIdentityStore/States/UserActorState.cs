namespace Hexalith.DaprIdentityStore.States;

using Hexalith.DaprIdentityStore.Models;

public class UserActorState
{
    public UserIdentity User { get; set; } = new();
}