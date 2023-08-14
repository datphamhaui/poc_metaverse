using Fusion;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public enum EInputButtons
{
    Dancing = 0,
    Flying = 1
}

public struct PlayerInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;
    public const byte MOUSEBUTTON2 = 0x02;
    public const uint FYLING = 1 << 6;
    public uint ubuttons;
    public byte buttons;
    public Vector2 MoveDirection;
    public NetworkButtons Buttons;
    public bool Dancing { get { return Buttons.IsSet(EInputButtons.Dancing); } set { Buttons.Set((int)EInputButtons.Dancing, value); } }
    public bool Flying { get { return Buttons.IsSet(EInputButtons.Flying); } set { Buttons.Set((int)EInputButtons.Flying, value); } }

    public bool IsDown(uint button)
    {
        return (ubuttons & button) == button;
    }
}

public class PlayerInput : SimulationBehaviour, ISpawned, IDespawned, IBeforeUpdate
{
    private PlayerInputData _cachedInput;
    private bool _resetCachedInput;
    private bool _mouseButton1;
    private bool _mouseButton2;
    public bool flyingReady { get; set; }

    private void Update()
    {
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(0);
        _mouseButton2 = _mouseButton2 || Input.GetMouseButton(1);
        flyingReady = flyingReady || Input.GetKeyDown(KeyCode.F);
    }

    void ISpawned.Spawned()
    {
        if (Runner.LocalPlayer == Object.InputAuthority)
        {
            var events = Runner.GetComponent<NetworkEvents>();

            events.OnInput.RemoveListener(OnInput);
            events.OnInput.AddListener(OnInput);
        }
    }

    void IDespawned.Despawned(NetworkRunner runner, bool hasState)
    {
        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.RemoveListener(OnInput);
    }

    void IBeforeUpdate.BeforeUpdate()
    {
        if (Object == null || Object.HasInputAuthority == false)
            return;

        if (_resetCachedInput == true)
        {
            _resetCachedInput = false;
            _cachedInput = default;
        }

        if (Runner.ProvideInput == false)
            return;

        ProcessKeyboardInput();
    }

    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        _resetCachedInput = true;
        networkInput.Set(_cachedInput);
    }

    private void ProcessKeyboardInput()
    {
        if (Input.GetKey(KeyCode.P) == true)
        {
            _cachedInput.Dancing = true;
        }

        if (flyingReady)
        {

            flyingReady = false;
            _cachedInput.ubuttons |= PlayerInputData.FYLING;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            _cachedInput.MoveDirection = new Vector2(horizontal, vertical);
        }

        if (_mouseButton1)
        {
            _cachedInput.buttons |= PlayerInputData.MOUSEBUTTON1;
        }
        if (_mouseButton2)
        {
            _cachedInput.buttons |= PlayerInputData.MOUSEBUTTON2;
        }

        _mouseButton1 = false;
        _mouseButton2 = false;
    }
}
