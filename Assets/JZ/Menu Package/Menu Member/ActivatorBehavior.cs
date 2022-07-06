using UnityEngine;

namespace JZ.MENU
{
    /// <summary>
    /// <para>Activates and deactivates objects when member is hovered over</para>
    /// </summary>
    public class ActivatorBehavior : MenuMemberBehavior
    {
        [SerializeField] private GameObject[] objectsToActivate = new GameObject[0];
        [SerializeField] private GameObject[] objectsToDeactivate = new GameObject[0];


        protected override void OnHover(bool active)
        {
            foreach(var go in objectsToActivate)
                go.SetActive(active);

            foreach(var go in objectsToDeactivate)
                go.SetActive(!active);
        }
    }
}