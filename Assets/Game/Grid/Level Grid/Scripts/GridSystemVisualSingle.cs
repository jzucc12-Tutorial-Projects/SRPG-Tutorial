using UnityEngine;

/// <summary>
/// Grid Cell UI element
/// </summary>
public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer = null;
    private Material storedMaterial = null;

    public void Show(Material material, bool store = true)
    {
        meshRenderer.enabled = true;
        if(store) storedMaterial = material;
        meshRenderer.material = material;
    }

    public void Restore()
    {
        if(storedMaterial != null) meshRenderer.material = storedMaterial;
        else Hide();
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
        storedMaterial = null;
    }

    public bool IsShowing() => meshRenderer.enabled;
}
