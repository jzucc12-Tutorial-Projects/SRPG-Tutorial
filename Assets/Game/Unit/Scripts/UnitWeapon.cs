using System;
using UnityEngine;

/// <summary>
/// Handles showing a unit's weapon
/// </summary>
public class UnitWeapon : MonoBehaviour
{
    [SerializeField] private GameObject defaultWeapon = null;
    private GameObject activeWeapon = null;
    public event Action<AnimatorOverrideController> OnWeaponSwap;


    private void Awake()
    {
        activeWeapon = defaultWeapon;
    }

    public void SetActiveWeapon(GameObject newWeapon, AnimatorOverrideController controller)
    {
        HideActiveWeapon();
        activeWeapon = newWeapon;
        if(controller != null) OnWeaponSwap?.Invoke(controller);
        ShowActiveWeapon();
    }

    public void ShowActiveWeapon()
    {
        activeWeapon.SetActive(true);
    }

    public void HideActiveWeapon()
    {
        activeWeapon.SetActive(false);
    }
}