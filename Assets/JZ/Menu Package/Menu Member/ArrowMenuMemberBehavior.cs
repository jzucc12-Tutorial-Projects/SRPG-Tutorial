using UnityEngine;
using UnityEngine.UI;

namespace JZ.MENU
{
    /// <summary>
    /// <para>Shows an arrow when hovering over the member</para>
    /// </summary>
    public class ArrowMenuMemberBehavior : MenuMemberBehavior
    {
        [SerializeField] private Image arrowImage = null;
        private Color arrowColor = Color.clear;


        protected override void Awake() 
        {
            arrowColor = arrowImage.color;
            base.Awake();
        }

        protected override void OnHover(bool isHovering)
        {
            Color newColor = arrowColor;
            newColor.a = isHovering ? arrowColor.a : 0;
            arrowImage.color = newColor;
        }
    }
}