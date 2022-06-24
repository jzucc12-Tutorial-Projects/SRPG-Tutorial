using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    #region //Variables
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Animator animator = null;
    private GridPosition gridPosition;
    private bool isActive = false;
    private float timer;
    private Action OnComplete;
    #endregion


    #region //Monobehavior
    private void Start()
    {
        gridPosition = LevelGrid.instance.GetGridPosition(transform.position);
        LevelGrid.instance.SetInteractableAtGridPosition(gridPosition, this);
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
    public void Interact(Action OnComplete)
    {
        if(isOpen) CloseDoor();
        else OpenDoor();
        isActive = true;
        timer = 0.5f;
        this.OnComplete = OnComplete;
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
        Pathfinding.instance.SetIsWalkable(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
        Pathfinding.instance.SetIsWalkable(gridPosition, false);
    }
    #endregion
}