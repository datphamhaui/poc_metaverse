using Cinemachine;
using Fusion;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class PlayerCharacter : NetworkBehaviour, ISimulationEnter, ISimulationExit
{
    [Networked, HideInInspector]
    public Vector2 direction { get; set; }

    [Networked, HideInInspector]
    public float speed { get; set; }
    public float interpolatedSpeed => _speedInterpolator.Value;

    private Interpolator<float> _speedInterpolator;
    [Networked]
    private NetworkButtons _lastButtonsInput { get; set; }

    public bool HasDancing { get; private set; }

    private PlayerMovement _playerMovement;

    public GameObject girl;

    public void SimulationEnter()
    {
        Debug.Log("SimulationEnter");
        girl.SetActive(true);
    }

    public void SimulationExit()
    {
        Debug.Log("SimulationExit");
        girl.SetActive(false);
    }

    public override void Spawned()
    {
        _speedInterpolator = GetInterpolator<float>(nameof(speed));
        _playerMovement = GetComponent<PlayerMovement>();
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
        HasDancing = input.Buttons.WasPressed(_lastButtonsInput, EInputButtons.Dancing) && speed <= .0f;
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
