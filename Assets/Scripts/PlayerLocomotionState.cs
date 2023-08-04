using UnityEngine;
using Fusion.Animations;

public class PlayerLocomotionState : BlendTreeState
{
    private PlayerCharacter _characterController;

    protected override Vector2 GetBlendPosition(bool interpolated)
    {
        return new Vector2(0.0f, interpolated == true ? _characterController.interpolatedSpeed : _characterController.speed);
    }

    private void Awake()
    {
        _characterController = GetComponentInParent<PlayerCharacter>();
    }
}

