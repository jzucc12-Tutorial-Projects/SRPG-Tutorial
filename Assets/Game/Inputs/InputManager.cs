#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;
    private PlayerInputs inputs = null;
    public InputAction mouseClick => inputs.Player.MouseClick;
    public InputAction altAction => inputs.Player.AltAction;


    #region //Monobehaviour
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);
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
    #endregion
}
