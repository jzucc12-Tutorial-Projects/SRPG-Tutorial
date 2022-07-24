using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace JZ.INPUT
{
    /// <summary>
    /// <para>Extra functions relating to player input</para>
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// <para>Converts a string into its corresponding key codes</para>
        /// </summary>
        /// <param name="keyCodeString"></param>
        /// <returns></returns>
        public static KeyCode[] KeyCodesFromString(string keyCodeString)
        {
            KeyCode[] keys = new KeyCode[keyCodeString.Length];
            for(int ii = 0; ii < keys.Length; ii++)
            {
                keys[ii] = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeString[ii].ToString());
            }

            return keys;
        }

        /// <summary>Checks if a key combo is being pressed
        /// <para>Returns true the frame the last key in the combo is pressed</para>
        /// </summary>
        /// <param name="keyCombo"></param>
        /// <returns></returns>
        public static bool CheckKeyCombo(string keyCombo)
        {
            KeyCode[] combo = KeyCodesFromString(keyCombo);
            for(int ii = 0; ii < combo.Length - 1; ii++)
                if(!Input.GetKey(combo[ii])) return false;

            return Input.GetKeyDown(combo[combo.Length - 1]);
        }

        /// <summary>Checks if a key combo is being pressed
        /// <para>Returns true the frame the last key in the combo is pressed</para>
        /// </summary>
        /// <param name="keyCombo"></param>
        /// <returns></returns>
        public static bool CheckKeyCombo(KeyCode[] keyCombo)
        {
            for(int ii = 0; ii < keyCombo.Length - 1; ii++)
                if(!Input.GetKey(keyCombo[ii])) return false;

            return Input.GetKeyDown(keyCombo[keyCombo.Length - 1]);
        }

        /// <summary>Returns true if a controller button is pressed
        /// </summary>
        public static bool AnyControllerButton()
        {
            if(Gamepad.current == null) return false;

            foreach(InputControl control in Gamepad.current.allControls)
            {
                if(!(control is ButtonControl)) continue;
                if(!control.IsPressed() || control.synthetic) continue;
                return true;
            }
            return false;
        }

        /// <summary>Returns true if a controller or computer button is pressed
        /// </summary>
        public static bool AnyKeyOrButton()
        {
            return Input.anyKey || AnyControllerButton();
        }
    }
}

