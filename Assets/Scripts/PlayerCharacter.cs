using Cinemachine;
using Fusion;
using Fusion.Animations;
using Photon.Chat.Demo;
using Photon.Realtime;
using POpusCodec.Enums;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class PlayerCharacter : NetworkBehaviour
{
    [Networked, HideInInspector]
    public Vector2 direction { get; set; }

    [Networked, HideInInspector]
    public float speed { get; set; }
    public float interpolatedSpeed => _speedInterpolator.Value;
    [Networked] private TickTimer delay { get; set; }

    private Interpolator<float> _speedInterpolator;
    [Networked]
    private NetworkButtons _lastButtonsInput { get; set; }
    [SerializeField] private NetworkHubHeath _heathHub;
    [SerializeField] private NetworkBloomOrb _bloomOrb;
    [SerializeField] private Transform _heathHubPosition;
    [SerializeField] private Transform _bloomOrbPosition;

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

    private void SpawnNetworkHubHeath()
    {
        Runner.Spawn(_heathHub, _heathHubPosition.position, Quaternion.identity, Object.InputAuthority, (runner, o) =>
        {
            o.GetBehaviour<NetworkHubHeath>().Init();
        });
    }

    private void SpawnNetworkBloomOrb()
    {
        Runner.Spawn(_bloomOrb, _bloomOrbPosition.position + transform.forward, Quaternion.LookRotation(transform.forward), Object.InputAuthority, (runner, o) =>
        {
            o.GetBehaviour<NetworkBloomOrb>().Init(transform.forward * 10.0f);
        });
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

        if (delay.ExpiredOrNotRunning(Runner))
        {
            if ((input.buttons & PlayerInputData.MOUSEBUTTON1) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 5.0f);
                SpawnNetworkHubHeath();
            }
            else if ((input.buttons & PlayerInputData.MOUSEBUTTON2) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 5.0f);
                SpawnNetworkBloomOrb();
            }
        }
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
