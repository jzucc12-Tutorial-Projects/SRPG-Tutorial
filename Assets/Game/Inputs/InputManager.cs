#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;
    private PlayerInputs inputs = null;


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
#if USE_NEW_INPUT_SYSTEM    
        return Mouse.current.position.ReadValue();
#else    
        return Input.mousePosition;
#endif
    }
    public bool GetMouseClick()
    {
#if USE_NEW_INPUT_SYSTEM  
        return inputs.Player.MouseClick.WasPerformedThisFrame();
#else    
        return Input.GetMouseButtonDown(0);
#endif
    }
    public Vector2 GetCameraMove()
    {
#if USE_NEW_INPUT_SYSTEM   
        return inputs.Player.CameraMove.ReadValue<Vector2>();
#else    
        Vector2 inputMoveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputMoveDir.y = 1;
        if (Input.GetKey(KeyCode.S)) inputMoveDir.y = -1;
        if (Input.GetKey(KeyCode.A)) inputMoveDir.x = -1;
        if (Input.GetKey(KeyCode.D)) inputMoveDir.x = 1;
        return inputMoveDir;
#endif
    }

    public float GetCameraRotate()
    {
#if USE_NEW_INPUT_SYSTEM    
        return inputs.Player.CameraRotate.ReadValue<float>();
#else    
        Vector2 rotationVector = Vector2.zero;
        if (Input.GetKey(KeyCode.Q)) return = 1;
        if (Input.GetKey(KeyCode.E)) return = -1;
#endif
    }

    public float GetCameraZoom()
    {
#if USE_NEW_INPUT_SYSTEM   
        return inputs.Player.CameraZoom.ReadValue<float>();
#else    
        if (Input.mouseScrollDelta.y > 0) return -1;
        if (Input.mouseScrollDelta.y < 0) return 1;
        return 0;
#endif
    }
    #endregion
}
