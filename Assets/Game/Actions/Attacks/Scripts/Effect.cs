using UnityEngine;

/// <summary>
/// Game effect that alerts the object pool to its being disabled
/// </summary>
public class Effect : MonoBehaviour
{
    private EffectPool pool;
    [SerializeField] private bool disableWithChild = false;
    [SerializeField, ShowIf("disableWithChild")] private GameObject child = null;


    public void SetPool(EffectPool pool) => this.pool = pool;

    private void OnEnable()
    {
        if(!disableWithChild) return;
        child.gameObject.SetActive(true);
    }

    private void OnDisable() 
    {
        ReleaseMe();
    }

    private void Update()
    {
        if(!disableWithChild) return;
        if(child.activeInHierarchy) return;
        gameObject.SetActive(false);
    }

    public void ReleaseMe() => pool.ReleaseEffect(this);
}