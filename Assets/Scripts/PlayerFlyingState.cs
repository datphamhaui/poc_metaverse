using UnityEngine;
using Fusion.Animations;

public class PlayerFlyingState : BlendTreeState
{
    private PlayerCharacter _characterController;

    protected override Vector2 GetBlendPosition(bool interpolated)
    {
        return new Vector2(_characterController.direction.x, _characterController.direction.y);
    }

    private void Awake()
    {
        _characterController = GetComponentInParent<PlayerCharacter>();
    }
}
