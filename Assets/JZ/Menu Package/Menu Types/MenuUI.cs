using UnityEngine;

namespace JZ.MENU.UI
{
    /// <summary>
    /// <para>Parent class for all menu types</para>
    /// </summary>
    public class MenuUI : MonoBehaviour
    {
        #region //Monobehaviour
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        #endregion

        public virtual void LockControl(bool shouldLock) { }
    }
}