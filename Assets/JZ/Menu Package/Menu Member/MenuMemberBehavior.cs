using UnityEngine;

namespace JZ.MENU
{
    /// <summary>
    /// <para>Parent class for effects that occur when you hover over menu members</para>
    /// </summary>
    public abstract class MenuMemberBehavior : MonoBehaviour
    {
        MenuMember myMember;

        protected virtual void Awake() 
        {
            myMember = GetComponent<MenuMember>();    
            OnHover(false);    
        }

        protected virtual void OnEnable() 
        {
            myMember.MemberHovered += OnHover;
        }

        protected virtual void OnDisable() 
        {
            myMember.MemberHovered -= OnHover;
        }

        protected virtual void OnHover(bool isHovering) { }
        protected virtual void OnSelect() { }
    }
}
