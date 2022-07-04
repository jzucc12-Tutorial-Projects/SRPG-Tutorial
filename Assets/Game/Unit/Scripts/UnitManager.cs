using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance = null;

    #region //Unit lists
    private List<Unit> unitList = new List<Unit>();
    private List<Unit> playerList = new List<Unit>();
    private List<Unit> enemyList = new List<Unit>();
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

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
        if(unit.IsEnemy()) 
        {
            enemyList.Add(unit);
            SortList(enemyList);
        }
        else
        {
            playerList.Add(unit);
            SortList(playerList);
        }
    }

    private void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
        if(unit.IsEnemy()) 
        {
            enemyList.Remove(unit);
            SortList(enemyList);
        }
        else 
        {
            playerList.Remove(unit);
            SortList(playerList);
        }
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
    public List<Unit> GetPlayerList() => playerList;
    public List<Unit> GetEnemyList() => enemyList;
    #endregion
}