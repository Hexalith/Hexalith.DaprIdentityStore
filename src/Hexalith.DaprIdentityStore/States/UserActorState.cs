namespace Hexalith.DaprIdentityStore.States;

public class UserActorState
{
    public ApplicationUserIdentity User { get; set; } = new();
}