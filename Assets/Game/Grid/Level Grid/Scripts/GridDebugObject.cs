using TMPro;
using UnityEngine;

/// <summary>
/// Shows debug info for a given grid object
/// </summary>
public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro position = null;
    private object gridObject = null;


    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        position.text = gridObject.ToString();
    }
}
