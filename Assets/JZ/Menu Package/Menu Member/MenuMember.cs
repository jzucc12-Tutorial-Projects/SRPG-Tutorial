using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JZ.MENU
{
    /// <summary>
    /// <para>Attach to items that you want to be recognized by a menu</para>
    /// </summary>
    public class MenuMember : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
    {
        #region //Variables
        public event Action<MenuMember> PointerEnter;
        public event Action<bool> MemberHovered;
        #endregion


        #region //Monobehaviour
        protected virtual void Awake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Start() 
        {
            Hover(false);    
        }
        #endregion

        #region //Event Methods
        public void Hover(bool active) 
        { 
            MemberHovered?.Invoke(active);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Removes focus from this element to prevent weird UI issues
            EventSystem.current.SetSelectedGameObject(null);
        }
        #endregion
    }
}