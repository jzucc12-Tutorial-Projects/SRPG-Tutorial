using System;
using UnityEngine;

/// <summary>
/// Interactable door
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    #region //Variables
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Animator animator = null;
    private bool isActive = false;
    private float timer;
    private Action OnComplete;
    #endregion


    #region //Monobehavior
    private void Start()
    {
        if(isOpen) OpenDoor();
        else CloseDoor();
    }

    private void Update()
    {
        if(!isActive) return;
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            OnComplete();
        }
    }
    #endregion

    #region //Interactions
    public void Interact(Unit actor, Action OnComplete)
    {
        string log;
        if(isOpen) 
        {
            log = $"{actor.GetName()} closed a door";
            CloseDoor();
        }
        else 
        {
            log = $"{actor.GetName()} opened a door";
            OpenDoor();
        }
        isActive = true;
        timer = 0.5f;
        this.OnComplete = OnComplete;

        ActionLogListener.Publish(log);
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
    }
    #endregion

    #region //Getters
    public Vector3 GetWorldPosition() => transform.position.PlaceOnGrid();
    public GridCell GetGridCell() => transform.position.GetGridCell();
    #endregion
}