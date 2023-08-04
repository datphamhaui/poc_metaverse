using Fusion;
using UnityEngine;

public struct PlayerInputData : INetworkInput
{
    public Vector2 MoveDirection;
}

public class PlayerInput : SimulationBehaviour, ISpawned, IDespawned, IBeforeUpdate
{
    private PlayerInputData _cachedInput;
    private bool _resetCachedInput;

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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            _cachedInput.MoveDirection = new Vector2(horizontal, vertical);
        }
    }
}
