using UnityEngine;

namespace JZ.MENU.UI
{
    /// <summary>
    /// <para>Menu that locks the menu that activated it</para>
    /// <para>Unlocks the parent upon close</para>
    /// </summary>
    public class SubMenu : MonoBehaviour
    {
        [SerializeField] private MenuUI parentMenu = null;
        [SerializeField] private GameObject blockingWindow = null;


        private void OnEnable()
        {
            blockingWindow.SetActive(true);
            parentMenu.LockControl(true);
        }

        private void OnDisable()
        {
            parentMenu.LockControl(false);
            blockingWindow.SetActive(false);
        }
    }
}
