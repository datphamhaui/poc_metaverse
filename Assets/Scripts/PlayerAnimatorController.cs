using Fusion;

public class PlayerAnimatorController : NetworkBehaviour
{
    private PlayerCharacter _playerController;
    private PlayerLocomotionState _playerLocomotionState;

    public override void FixedUpdateNetwork()
    {
        if (IsProxy == true)
            return;
        _playerLocomotionState.Activate(0.15f);
    }

    protected void Awake()
    {
        _playerController = GetComponentInChildren<PlayerCharacter>();
        _playerLocomotionState = GetComponentInChildren<PlayerLocomotionState>();
    }
}
