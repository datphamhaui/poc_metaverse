using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Vector3 _direction;
    private CharacterController _characterController;

    private float _gravity = -9.810f;
    private float _gravityMultiplier = 3.0f;
    [SerializeField] private float _velocity;

    private float _inputX;
    private float _inputY;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float desiredRotationSpeed = 0.1f;
    private float EPSILON_SQR = 0.0001f;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        CursorConfig();
        CameraSetup();
    }

    private void Start()
    {

    }

    private void Update()
    {
        InputProcess();
        DirectionCalculator();
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void CursorConfig()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CameraSetup() 
    {
        CinemachineFreeLook freelookCamera = FindObjectOfType<CinemachineFreeLook>();

        if (freelookCamera != null ) 
        {
            freelookCamera.LookAt = transform;
            freelookCamera.Follow = transform;
        }
    }

    private void InputProcess()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputY = Input.GetAxis("Vertical");
    }

    private void DirectionCalculator()
    {
        var forward = _mainCamera.transform.forward;
        var right = _mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        _direction = forward * _inputY + right * _inputX;
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity < .0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * _gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyRotation()
    {
        Vector3 xzRotation = new Vector3(_direction.x, .0f, _direction.z);

        if (xzRotation.sqrMagnitude > EPSILON_SQR)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(xzRotation), desiredRotationSpeed);
        }
    }

    private void ApplyMovement()
    {
        _characterController.Move(_direction * Time.deltaTime * _speed);
    }
}
