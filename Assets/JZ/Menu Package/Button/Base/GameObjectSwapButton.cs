using UnityEngine;

namespace JZ.MENU.BUTTON
{
    /// <summary>
    /// <para>Deactivates on game object while activating another</para>
    /// </summary>
    public class GameObjectSwapButton : ButtonFunction
    {
        [SerializeField] private GameObject goToClose = null;
        [SerializeField] private GameObject goToOpen = null;
        public override void OnClick()
        {
            goToClose.SetActive(false);
            goToOpen.SetActive(true);
        }
    }
}
