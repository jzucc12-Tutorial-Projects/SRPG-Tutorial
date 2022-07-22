using UnityEngine;

public class ToggleFullscreen : MonoBehaviour
{
    void Update()
    {
        var ctrlDown = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if(Input.GetKey(KeyCode.F) && ctrlDown)
            Screen.fullScreen = !Screen.fullScreen;
        else if(Input.GetKeyDown(KeyCode.F) && ctrl)
            Screen.fullScreen = !Screen.fullScreen;
    }
}
