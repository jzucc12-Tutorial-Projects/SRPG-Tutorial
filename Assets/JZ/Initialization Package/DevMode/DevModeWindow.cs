using UnityEngine;

namespace JZ.DEVMODE
{
    /// <summary>
    /// <para>Pop up window only available in dev mode</para>
    /// </summary>
    public class DevModeWindow : MonoBehaviour
    {
        [SerializeField] KeyCode[] keyCodes = new KeyCode[0];
        [SerializeField] GameObject devModeWindow = null;


        private void Awake()
        {
            enabled = DevMode.InDevMode();
        }

        private void Update()
        {
            if(JZ.INPUT.Utils.CheckKeyCombo(keyCodes))
            {
                devModeWindow.SetActive(!devModeWindow.activeInHierarchy);
            }
        }
    }
}
