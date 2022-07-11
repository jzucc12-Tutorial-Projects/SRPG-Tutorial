#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input
/// </summary>
public class InputManager : MonoBehaviour
{
    private PlayerInputs inputs = null;
    public InputAction mouseClick => inputs.Player.MouseClick;
    public InputAction changeTeammates => inputs.Player.ChangeTeammates;
    public InputAction doubleClick => inputs.Player.DoubleClick;


    #region //Monobehaviour
    private void Awake()
    {
        inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
    #endregion

    #region //Inputs
    public Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public Vector2 GetCameraMove()
    {
        return inputs.Player.CameraMove.ReadValue<Vector2>();
    }

    public float GetCameraRotate()
    {
        return inputs.Player.CameraRotate.ReadValue<float>();
    }

    public float GetCameraZoom()
    {
        return inputs.Player.CameraZoom.ReadValue<float>();
    }

    public float GetTeammateChange()
    {
        return changeTeammates.ReadValue<float>();
    }
    #endregion
}
