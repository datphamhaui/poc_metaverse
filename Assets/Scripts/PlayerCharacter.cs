using Cinemachine;
using Fusion;
using Fusion.Animations;
using Photon.Chat.Demo;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFormChanged))]
    public NetworkBool readyToFyling { get; set; }
    public static void OnFormChanged(Changed<NetworkBehaviour> changed)
    {
        ((PlayerCharacter)changed.Behaviour)?.OnFormChanged();
    }

    private void OnFormChanged()
    {

        foreach (var particle in _twinkleParticles)
        {
            if (particle != null)
            {
                particle.SetActive(readyToFyling);
            }
        }
    }

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
    [SerializeField] private GameObject _vrUI;
    [SerializeField] private GameObject _cameraMain;
    [SerializeField] GameObject[] _twinkleParticles;

    bool _stopMove;
    public bool hasDancing { get; private set; }

    private NetworkCulling _networkCulling;
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private AnimationController _animationController;
    [SerializeField] private GameObject _characterMeshObjects;

    // Npc handle
    private float interactionRange = 5.0f; // The range at which the player can interact with the NPC.
    private GameObject npc; // The NPC GameObject.
    //[SerializeField] private GameObject ;

    bool IsNearNPC()
    {
        float distance = Vector3.Distance(transform.position, npc.transform.position);
        return distance <= interactionRange;
    }


    private void Awake()
    {
        _networkCulling = GetComponent<NetworkCulling>();
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animationController = GetComponent<AnimationController>();
        _networkCulling.updated += OnCullingUpdated;
        _vrUI = GameObject.Find("VR-UI");
        _cameraMain = Camera.main.gameObject;
        npc = GameObject.Find("NPC_Character");
        if (_vrUI != null)
        {
            _vrUI.SetActive(false);
        }
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

    private void Update()
    {

    }

    private void OpenVRUI()
    {
        if (_vrUI != null)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                _stopMove = true;
                _vrUI.SetActive(true);
                _cameraMain.SetActive(false);
                _playerMovement.CursorLock(false);
            }
            else if (UnityEngine.Input.GetKeyUp(KeyCode.E))
            {
                _stopMove = false;
                _vrUI.SetActive(false);
                _cameraMain.SetActive(true);
                _playerMovement.CursorLock();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsProxy == true)
            return;

        var input = GetInput<PlayerInputData>();

        if (input.HasValue == true && !_stopMove)
        {
            ProcessInput(input.Value);
        }

        if (IsNearNPC())
        {
            //NPCController npcController = npc.GetComponent<NPCController>();
            //dialogueText.text = npcController.dialogue;
            Debug.Log("IsNearNPC");
        }
    }

    private void ProcessInput(PlayerInputData input)
    {
        speed = input.MoveDirection.sqrMagnitude;
        direction = input.MoveDirection;
        hasDancing = input.Buttons.WasPressed(_lastButtonsInput, EInputButtons.Dancing) && speed <= .0f;
        if (input.IsDown(PlayerInputData.FYLING))
        {
            if (Object.HasInputAuthority)
            {
                readyToFyling = !readyToFyling;
            }
        }
    }

    private void InitBalllAndHub(PlayerInputData input)
    {
        if (delay.ExpiredOrNotRunning(Runner))
        {
            if ((input.buttons & PlayerInputData.MOUSEBUTTON1) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 1.0f);
                SpawnNetworkBloomOrb();
            }
            else if ((input.buttons & PlayerInputData.MOUSEBUTTON2) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 2.0f);
                SpawnNetworkHubHeath();
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
