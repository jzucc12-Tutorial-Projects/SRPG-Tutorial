using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JZ.MENU.BUTTON
{
    /// <summary>
    /// <para>Extension of the Button class that requires the button to be held</para>
    /// </summary>
    public class HoldButton : Button
    {
        #region //Variables
        [SerializeField] private float timeToHold = 2f;
        private float currHoldtimer = 0;
        #endregion

        #region //Pointer events
        public override void OnPointerDown(PointerEventData eventData)
        {
            eventData.selectedObject = null;
            StartCoroutine(HoldCount());
        }

        public override void OnPointerClick(PointerEventData eventData) { } 

        public override void OnPointerUp(PointerEventData eventData)
        {
            currHoldtimer = 0;
            StopAllCoroutines();
        }
        #endregion

        #region //Hold methods
        private IEnumerator HoldCount()
        {
            while(currHoldtimer < timeToHold)
            {
                currHoldtimer += Time.deltaTime;
                yield return null;
            }
            onClick.Invoke();
        }

        public float GetProgressPercentage() => currHoldtimer / timeToHold;
        #endregion
    
        public float GetHoldTimer() { return timeToHold; }
    }
}