using UnityEngine;
using UnityEngine.UI;

namespace JZ.MENU.BUTTON
{
    /// <summary>
    /// <para>Parent class that automatically attaches functionality to a button without
    /// needing to use the inspector</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class ButtonFunction : MonoBehaviour
    {
        #region//Monobehaviour
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }
        #endregion

        #region//Pointer events
        public abstract void OnClick();
        #endregion
    }
}