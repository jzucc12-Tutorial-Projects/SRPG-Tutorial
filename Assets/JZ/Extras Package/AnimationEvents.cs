using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent event1;
    [SerializeField] private UnityEvent event2;
    [SerializeField] private UnityEvent event3;
    

    public void ActivateEvent(EventNumber eventNum)
    {
        switch(eventNum)
        {
            case EventNumber.event1:
                event1?.Invoke();
                break;
            case EventNumber.event2:
                event2?.Invoke();
                break;
            case EventNumber.event3:
                event3?.Invoke();
                break;
            default:
                Debug.Log(eventNum + " is an invalid input");
                break;
        }
    }

    public enum EventNumber
    {
        event1,
        event2,
        event3
    }
}
