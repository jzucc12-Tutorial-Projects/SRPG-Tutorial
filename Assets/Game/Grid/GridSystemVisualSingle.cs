using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer = null;

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
