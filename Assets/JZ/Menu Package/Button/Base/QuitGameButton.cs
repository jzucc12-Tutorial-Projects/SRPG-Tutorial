using UnityEngine;

namespace JZ.MENU.BUTTON
{
    /// <summary>
    /// <para>Closes the application</para>
    /// <para>Not available in web builds</para>
    /// </summary>
    public class QuitGameButton : ButtonFunction
    {
        protected override void Awake()
        {
            #if UNITY_WEBGL
            gameObject.SetActive(false);
            #else
            base.Awake();
            #endif
        }

        public override void OnClick()
        {
            Application.Quit();
        }
    }
}