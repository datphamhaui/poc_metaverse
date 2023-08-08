using Cinemachine;
using Fusion;
using Fusion.Animations;
using Photon.Chat.Demo;
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
    [Networked]
    private NetworkButtons _lastButtonsInput { get; set; }

    public bool HasDancing { get; private set; }
    private NetworkCulling _networkCulling;
    private PlayerMovement _playerMovement;
    private CharacterController _characterController;
    private AnimationController _animationController;
    [SerializeField] private GameObject _characterMeshObjects;

    private void Awake()
    {
        _networkCulling = GetComponent<NetworkCulling>();
        _characterController = GetComponent<CharacterController>();
        _animationController = GetComponent<AnimationController>();
        _networkCulling.updated += OnCullingUpdated;
    }

    private void OnCullingUpdated(bool isCulled)
    {
        bool isActive = isCulled == false;

        _characterMeshObjects.SetActive(isActive);
    }
    public override void Spawned()
    {
        _speedInterpolator = GetInterpolator<float>(nameof(speed));
        _playerMovement = GetComponent<PlayerMovement>();

        CameraSetup();

        if (!IsProxy)
        {
            NamePickGui chatgui = FindObjectOfType<NamePickGui>();
            if (chatgui != null)
            {
                //chatgui.ShowConnectPanel();
                chatgui.StartChat();
            }
        }
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
