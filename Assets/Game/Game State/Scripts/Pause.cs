using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
    #region //Variables
    [SerializeField] GameObject pauseUI = null;
    public static event Action<bool> OnPause;
    private bool isPaused = false;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        pauseUI.SetActive(false);
    }

    private void OnEnable()
    {
        UnitManager.GameOver += Halt;
    }

    private void OnDisable()
    {
        UnitManager.GameOver -= Halt;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }
    #endregion

    #region //Change Pause
    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        OnPause?.Invoke(isPaused);
    }

    private void Halt()
    {
        if(isPaused) TogglePause();
        enabled = false;
    }
    #endregion
}
