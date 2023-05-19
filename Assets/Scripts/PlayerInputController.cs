using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const string MOUSE_AXIS_X = "Mouse X";
    private const string MOUSE_AXIS_Y = "Mouse Y";
    private const string JUMP_BUTTON = "Jump";
    private float mouseSensitivityX = 1;
    private float mouseSensitivityY = 2;

    public NetworkInputData CheckInput()
    {
        var dataInput = new NetworkInputData();
        dataInput.AxisHorizontal = Input.GetAxis(HORIZONTAL_AXIS);
        dataInput.AxisVertical = Input.GetAxis(VERTICAL_AXIS);
        dataInput.MouseAxisX = Input.GetAxis(MOUSE_AXIS_X) * mouseSensitivityX;
        dataInput.MouseAxisY = Input.GetAxis(MOUSE_AXIS_Y) * mouseSensitivityY;
        dataInput.IsHoldingLeftShift = Input.GetKey(KeyCode.LeftShift);
        return dataInput;
    }
}
