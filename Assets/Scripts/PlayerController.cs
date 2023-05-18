using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject _mesh;
    private Transform _camera;
    [Networked, HideInInspector]
    public int JumpCount { get; set; }
    [Networked, HideInInspector]
    public float Speed { get; set; }

    public bool HasJumped { get; private set; }
    public bool HasRunned { get; private set; }

    public int InterpolatedJumpCount => _jumpCountInterpolator.Value;
    public float InterpolatedSpeed => _speedInterpolator.Value;

    private NetworkCharacterControllerPrototype _cc;

    [Networked]
    private NetworkButtons _lastButtonsInput { get; set; }

    private Interpolator<int> _jumpCountInterpolator;
    private Interpolator<float> _speedInterpolator;
    private void Awake()
    {
        _jumpCountInterpolator = GetInterpolator<int>(nameof(JumpCount));
        _speedInterpolator = GetInterpolator<float>(nameof(Speed));
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * Runner.DeltaTime * data.direction);
            HasJumped = data.Buttons.WasPressed(_lastButtonsInput, EInputButtons.Jump);

            if (data.speed == 3)
            {
                Speed = 3;
                Debug.Log("True " + Speed);
            }
            else
            {
                Speed = data.direction.magnitude;
                Debug.Log("False " + Speed);

            }

            if (HasJumped == true)
            {
                JumpCount++;
                Debug.Log("JumpCount " + JumpCount);
            }

            if (Object.HasInputAuthority)
            {
                if (_camera == null)
                    _camera = Camera.main.transform;
                Transform t = _mesh.transform;
                Vector3 p = t.position;
                _camera.position = p - 5 * t.forward + 5 * Vector3.up;
                Debug.Log("_camera.position " + _camera.position);
                _camera.LookAt(p + 2 * Vector3.up);
            }


            // In reality speed can probably be just taken from already existing
            // networked data (e.g. from KCC addon - kcc.Data.RealSpeed) so it
            // won't be necessary to store it anywhere else.

            _lastButtonsInput = data.Buttons;

        }
    }
}
