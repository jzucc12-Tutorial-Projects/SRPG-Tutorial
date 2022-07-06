using UnityEngine;

namespace JZ.MENU
{
    /// <summary>
    /// <para>Enables and disables components when hovered over</para>
    /// </summary>
    public class EnablerBehavior : MenuMemberBehavior
    {
        [SerializeField] private MonoBehaviour[] behavioursToEnable = new MonoBehaviour[0];
        [SerializeField] private MonoBehaviour[] behavioursToDisable = new MonoBehaviour[0];


        protected override void OnHover(bool isHovering)
        {
            foreach(var behaviour in behavioursToEnable)
                behaviour.enabled = isHovering;

            foreach(var behaviour in behavioursToDisable)
                behaviour.enabled = !isHovering;
        }
    }
}