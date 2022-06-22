using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject = null;
    [SerializeField] private TextMeshPro tmp = null;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    private void Update()
    {
        tmp.text = gridObject.ToString();
    }
}
