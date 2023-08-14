using Fusion;
using Fusion.Animations;
using UnityEngine;

public class PlayerAnimatorController : NetworkBehaviour
{
    private PlayerCharacter _playerController;
    private PlayerInput _playerInput;
    private PlayerLocomotionState _playerLocomotionState;
    private PlayerDancingState _playerDancingState;
    private PlayerFlyingState _playerFlyingState;
    public override void FixedUpdateNetwork()
    {
        if (IsProxy == true)
            return;

        if (_playerController.hasDancing == true)
        {
            _playerDancingState.Activate(0.15f);
        }

        else if (_playerController.readyToFyling == true)
        {
            _playerFlyingState.Activate(0.15f);
        }
        //else if (_playerDancingState.IsPlaying() == false || _playerDancingState.IsFinished(-0.15f, false) == true)
        else if (_playerDancingState.IsActive() == false || _playerController.speed > .00001f)
        {
            _playerLocomotionState.Activate(0.15f);
        }
    }

    protected void Awake()
    {
        _playerController = GetComponentInChildren<PlayerCharacter>();
        _playerInput = GetComponentInChildren<PlayerInput>();
        _playerLocomotionState = GetComponentInChildren<PlayerLocomotionState>();
        _playerDancingState = GetComponentInChildren<PlayerDancingState>();
        _playerFlyingState = GetComponentInChildren<PlayerFlyingState>();
    }
}
