using UnityEngine;

/// <summary>
/// Container for all game effects
/// </summary>
public class EffectsManager : MonoBehaviour
{
    [SerializeField] EffectPool bulletPool = null;
    [SerializeField] EffectPool grenadePool = null;
    [SerializeField] EffectPool manaBoltPool = null;
    [SerializeField] EffectPool healingWindPool = null;
    [SerializeField] EffectPool fireballPool = null;
    [SerializeField] EffectPool lightningBoltPool = null;
    [SerializeField] EffectPool lifeDrainPool = null;


    public Bullet GetBullet()
    {
        return bulletPool.GetEffect().GetComponent<Bullet>();
    }

    public Grenade GetGrenade()
    {
        return grenadePool.GetEffect().GetComponent<Grenade>();
    }

    public Bullet GetManaBolt()
    {
        return manaBoltPool.GetEffect().GetComponent<Bullet>();
    }

    public GameObject GetHealingWind()
    {
        return healingWindPool.GetEffect().gameObject;
    }

    public ParticleSystem GetFireball()
    {
        return fireballPool.GetEffect().GetComponent<ParticleSystem>();
    }

    public LightningBolt GetLightningBolt()
    {
        return lightningBoltPool.GetEffect().GetComponent<LightningBolt>();
    }

    public Bullet GetLifeDrain()
    {
        return lifeDrainPool.GetEffect().GetComponent<Bullet>();
    }

    public Grenade GetGrenadeReference() => grenadePool.GetReference().GetComponent<Grenade>();
}
