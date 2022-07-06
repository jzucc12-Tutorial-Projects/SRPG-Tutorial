using System;
using UnityEngine;

/// <summary>
/// Interactable light switch. Used for debugging
/// </summary>
public class LightSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material onMaterial = null;
    [SerializeField] private Material offMaterial = null;
    [SerializeField] private MeshRenderer mesh = null;
    [SerializeField] private bool isOn = true;


    private void Start()
    {
        SwitchOn(isOn);
    }

    public void Interact(Unit actor, Action OnComplete)
    {
        string log;
        SwitchOn(!isOn);
        if(isOn) log = $"{actor.GetName()} turned on a light";
        else log = $"{actor.GetName()} turned off a light";
        OnComplete();

        ActionLogListener.Publish(log); 
    }

    private void SwitchOn(bool isOn)
    {
        this.isOn = isOn;
        mesh.material = isOn ? onMaterial : offMaterial;
    }

    public Vector3 GetWorldPosition() => transform.position.PlaceOnGrid();

    public GridCell GetGridCell() => transform.position.GetGridCell();
}