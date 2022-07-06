using UnityEngine;

/// <summary>
/// Handles UI for showing if a unit is currently selected by the player
/// </summary>
public class UnitSelectedVisual : MonoBehaviour
{
    #region //Variables
    [SerializeField] private Unit unit = null;
    private MeshRenderer meshRenderer = null;
    private UnitActionSystem unitActionSystem = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        unitActionSystem = FindObjectOfType<UnitActionSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisual(false);
    }

    private void OnEnable()
    {
        UnitActionSystem.OnSelectedUnitChanged += ChangeSelected;
    }

    private void OnDisable()
    {
        UnitActionSystem.OnSelectedUnitChanged -= ChangeSelected;
    }
    #endregion

    #region //Updating visuals
    private void ChangeSelected(Unit newUnit)
    {
        UpdateVisual(newUnit == unit);
    }

    private void UpdateVisual(bool activate)
    {
        meshRenderer.enabled = activate;
    }
    #endregion
}