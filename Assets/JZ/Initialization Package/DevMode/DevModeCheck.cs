using UnityEngine;

namespace JZ.DEVMODE
{
    /// <summary>
    /// <para>Changes object activation in relation to dev mode</para>
    /// </summary>
    public class DevModeCheck : MonoBehaviour
    {
        [SerializeField] private bool matchDev = true;

        private void Awake() 
        {
            gameObject.SetActive(!(DevMode.InDevMode() ^ matchDev));    
        }
    }
}