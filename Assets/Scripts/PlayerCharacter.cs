using Cinemachine;
using Fusion;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class PlayerCharacter : NetworkBehaviour
{
    [Networked, HideInInspector]
    public Vector2 direction { get; set; }

    [Networked, HideInInspector]
    public float speed { get; set; }
    public float interpolatedSpeed => _speedInterpolator.Value;

    private Interpolator<float> _speedInterpolator;

    public override void Spawned()
    {
        _speedInterpolator = GetInterpolator<float>(nameof(speed));
        CameraSetup();
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
        speed = input.MoveDirection.sqrMagnitude;
        direction = input.MoveDirection;
    }

    private void CameraSetup()
    {
        if (Object.HasInputAuthority)
        {
            CinemachineFreeLook freelookCamera = FindObjectOfType<CinemachineFreeLook>();

            if (freelookCamera != null)
            {
                freelookCamera.LookAt = transform;
                freelookCamera.Follow = transform;
            }
        }
    }
}
