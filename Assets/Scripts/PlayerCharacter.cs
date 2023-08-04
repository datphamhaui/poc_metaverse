using Fusion;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class PlayerCharacter : NetworkBehaviour
{
    [Networked, HideInInspector]
    public float speed { get; set; }
    public float interpolatedSpeed => _speedInterpolator.Value;

    private Interpolator<float> _speedInterpolator;

    public override void Spawned()
    {
        _speedInterpolator = GetInterpolator<float>(nameof(speed));
    }

    public override void FixedUpdateNetwork()
    {
        if (IsProxy == true)
            return;

        var input = GetInput<PlayerInputData>();

        if (input.HasValue == true)
        {
            ProcessInput(input.Value);
        }
    }

    private void ProcessInput(PlayerInputData input)
    {
        speed = input.MoveDirection.magnitude;
    }
}
