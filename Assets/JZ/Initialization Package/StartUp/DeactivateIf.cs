using JZ.DEVMODE;
using UnityEngine;

namespace JZ.STARTUP
{
    /// <summary>
    /// <para>Changes object activation if a certain prerequesite is true</para>
    /// </summary>
    public abstract class DeactivateIf : MonoBehaviour
    {
        [SerializeField] private bool useDefaultInDevMode = true;
        [SerializeField, ShowIf("useDefaultInDevMode")] private bool defaultValue = true;


        private void Awake()
        {
            SetActive(DeactivateCheck());
        }

        private void SetActive(bool active)
        {
            if (useDefaultInDevMode && DevMode.InDevMode())
                gameObject.SetActive(defaultValue);
            else
                gameObject.SetActive(active);
        }

        protected abstract bool DeactivateCheck();
    }
}