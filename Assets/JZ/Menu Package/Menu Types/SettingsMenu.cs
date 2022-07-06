using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.MENU.UI
{
    /// <summary>
    /// <para>Controls the settings menu</para>
    /// </summary>
    public class SettingsMenu : MenuUI
    {
        #region //Variables
        [SerializeField] private GameObject typeSelector = null;
        private Button[] typeButtons = null;

        [Tooltip("Menu order should match the button order in the type selector")] 
        [SerializeField] private GameObject[] menus = new GameObject[0];
        private Dictionary<Button, GameObject> typeDictionary = new Dictionary<Button, GameObject>();
        #endregion
        

        #region //Monobehaviour
        protected override void Awake()
        {
            base.Awake();
            typeButtons = typeSelector.GetComponentsInChildren<Button>();
            SetUpDictionary();
            SettingTypeChange(typeButtons[0]);
        }

        protected override void OnDisable()
        {
            SettingTypeChange(typeButtons[0]);
        }
        #endregion

        #region //Activation
        public override void LockControl(bool shouldLock)
        {
            foreach(var key in typeDictionary.Keys)
                key.interactable = !shouldLock;
        }
        private void SetUpDictionary()
        {
            if(menus.Length != typeButtons.Length)
            {
                Debug.LogError("Number of settings menus don't match the number of settings buttons");
            }
            else
            {
                for (int ii = 0; ii < typeButtons.Length; ii++)
                {
                    typeDictionary.Add(typeButtons[ii], menus[ii]);
                }
            }
        }
        #endregion

        #region //Changing setting type
        public void SettingTypeChange(Button pressedButton)
        {
            //Reset entries
            foreach(var pair in typeDictionary)
            {
                pair.Key.interactable = true;
                pair.Value.SetActive(false);
            }

            //Activate pressed entry
            pressedButton.interactable = false;
            typeDictionary[pressedButton].SetActive(true);
        }
        #endregion
    }
}