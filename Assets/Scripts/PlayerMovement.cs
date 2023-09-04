using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Vector3 _direction;
    private CharacterController _characterController;
    private PlayerCharacter _playerCharacter;

    private float _gravity = -9.810f;
    private float _gravityMultiplier = 1.50f;
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
        _playerCharacter = GetComponent<PlayerCharacter>();
        //CursorLock();
    }

    private void Update()
    {
        InputProcess();
        DirectionCalculator();
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }

    }

    public void CursorLock(bool condition = true)
    {
        if (condition)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void InputProcess()
    {
        _inputX = _playerCharacter.direction.x;
        _inputY = _playerCharacter.direction.y;
    }

    private void DirectionCalculator()
    {
        var forward = _mainCamera.transform.forward;
        var right = _mainCamera.transform.right;

        if (_playerCharacter.readyToFyling)
        {
            _inputY *= 2.0f;
        }
        else
        {
            forward.y = 0f;
            right.y = 0f;
        }

        forward.Normalize();
        right.Normalize();

        _direction = forward * _inputY + right * _inputX;
    }

    private void ApplyGravity()
    {
        if (_playerCharacter.readyToFyling) return;

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
