using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Object pool for game effects
/// </summary>
public class EffectPool : MonoBehaviour
{
    [SerializeField] Effect effect = null;
    [SerializeField] private int defaultSize = 2;
    [SerializeField] private int maxSize = 5;
    ObjectPool<Effect> effectPool = null;


    private void Awake()
    {
         effectPool = new ObjectPool<Effect>(Create, OnGet, OnReturn, Destroyed, false, defaultSize, maxSize);

         var objs = new List<Effect>();
         for(int ii = 0; ii < defaultSize; ii++)
            objs.Add(effectPool.Get());

         foreach(var obj in objs)
            effectPool.Release(obj);
    }

    private Effect Create()
    {
        var obj = Instantiate(effect, transform);
        obj.SetPool(this);
        return obj;
    }

    private void OnGet(Effect obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReturn(Effect obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void Destroyed(Effect obj)
    {
        Destroy(obj);
    }

    public Effect GetEffect() => effectPool.Get();
    public void ReleaseEffect(Effect effect) => effectPool.Release(effect);
}