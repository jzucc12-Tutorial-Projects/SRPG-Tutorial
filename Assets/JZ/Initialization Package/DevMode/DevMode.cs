using UnityEngine;

namespace JZ.DEVMODE
{
    /// <summary>
    /// <para>Sets whether or not the game is in dev mode</para>
    /// </summary>
    public class DevMode : MonoBehaviour
    {
        public const string devModeKey = "inDevMode";
        [SerializeField] private bool devMode = false;

        private void Awake()
        {
            int devModeValue = devMode ? 1 : 0;
            PlayerPrefs.SetInt(devModeKey, devModeValue);
        }

        public static bool InDevMode()
        {
            int devModeValue = PlayerPrefs.GetInt(devModeKey, 0);
            return devModeValue == 1;
        }
    }

}