using System;
using System.Collections.Generic;
using UnityEngine;
using JZ.MATH;

/// <summary>
/// Keeps track of all living units.
/// </summary>
public class UnitManager : MonoBehaviour
{
    #region //Unit lists
    private List<Unit> unitList = new List<Unit>();
    private List<Unit> team1List = new List<Unit>();
    private List<Unit> team2List = new List<Unit>();
    public static event Action<bool> GameOverSided; //True if team 1 wins
    public static event Action GameOver;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        Unit.UnitSpawned += AddUnit;
        Unit.UnitDead += RemoveUnit;
    }

    private void OnDisable()
    {
        Unit.UnitSpawned -= AddUnit;
        Unit.UnitDead -= RemoveUnit;
    }
    #endregion

    #region //List changing
    private void AddUnit(Unit unit)
    {
        unitList.Add(unit);
        if(unit.IsTeam1()) 
        {
            team1List.Add(unit);
            SortList(team1List);
        }
        else
        {
            team2List.Add(unit);
            SortList(team2List);
        }
    }

    private void RemoveUnit(Unit unit)
    {
        if(unit.IsTeam1()) 
            RemoveFromList(team1List, unit);
        else 
            RemoveFromList(team2List, unit);
    }

    private void RemoveFromList(List<Unit> list, Unit unit)
    {
        unitList.Remove(unit);
        list.Remove(unit);
        SortList(list);
        if(list.Count > 0) return;
        GameOver?.Invoke();
        GameOverSided?.Invoke(!unit.IsTeam1());
        Time.timeScale = 0.2f;
    }

    private void SortList(List<Unit> list)
    {
        list.Sort((Unit x, Unit y) =>
        {
            if(x.GetPriority() > y.GetPriority()) return 1;
            else if(x.GetPriority() < y.GetPriority()) return -1;
            return 0;
        });
    }
    #endregion

    #region //Getters
    public List<Unit> GetUnitList() => unitList;
    public Unit GetRootUnit(bool isTeam1)
    {
        var list = isTeam1 ? team1List : team2List;
        return list.Count > 0 ? list[0] : null;
    }

    public Unit GetShiftUnit(Unit unit, bool isTeam1, int shift)
    {
        var list = isTeam1 ? team1List : team2List;
        int currentIndex = list.IndexOf(unit);
        int newIndex = Utils.Wrap(currentIndex + shift, 0, list.Count - 1);
        return list[newIndex];
    }
    #endregion
}