using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// LIMITATIONS:
/// The AI will either move first, last, or not at all. It won't act, move, and then act
/// The AI will not move multiple times in a given turn
/// Currently the AI favors movements that place it further away from enemies. Playing passively.
/// AI can't track current ammo, stock, etc when choosing an action
/// The AI will replan its turn after every action it takes.

/// POSSIBLE CHANGES:
/// Make handling movement better. I first need to decide if I want there to be a limit on one or two movements per turn. Scoring moving is also weird.
///     Reason choosing between being aggressive (wanting to be near foes) or being defensive (wanting to get away from foes). I could potentially
///     make it dependent upon remaining AP, health, and a few other factors. TBD.
/// Possibly have the AI consider the new plan rather than always take it

/// <summary>
/// Decides and take the turn for an enemy ai
/// </summary>
[RequireComponent(typeof(Unit))]
public class EnemyAI : MonoBehaviour
{
    private Unit enemy = null;
    private MoveAction moveAction = null;


    #region //Monobehaviour
    private void Awake()
    {
        enemy = GetComponent<Unit>();
        moveAction = enemy.GetComponent<MoveAction>();
        if(GameGlobals.TwoPlayer()) enabled = false;
    }
    #endregion

    #region //Take turn
    public IEnumerator TakeTurn(float waitTime)
    {
        //Set up
        bool waiting = false;
        Action actionComplete = () => waiting = false;
        var actionList = DetermineActions();
        Func<bool> doneWaiting = () => !waiting || !enemy.IsAlive();
        int actionNo = 0;

        while(enemy.GetAP() > 0)
        {
            //Reset
            bool performed = false;
            waiting = true;
            
            //Get next action. Abort if out of actions
            EnemyAIAction aiAction = actionList.GetAction(actionNo);
            if(aiAction == null) break;

            //Take the current action if possible
            if(enemy.TryTakeAction(aiAction.action, aiAction.targetCell))
            {
                performed = true;
                actionNo++;
                aiAction.PerformAction(actionComplete);
                yield return new WaitUntil(doneWaiting);
                if(!enemy.IsAlive()) yield break;
                yield return new WaitForSeconds(waitTime);
            }

            //Update list with current state
            var testList = MakeActionList(new EnemyAIActionList(actionList, actionNo), enemy.GetGridCell(), enemy.GetAP());
            if(!performed && testList == actionList) break;
            actionList = testList;
        }
    }

    /// <summary>
    /// Old method of how to handle an action being inelligible
    /// </summary>
    /// <param name="aiAction"></param>
    /// <returns></returns>
    private EnemyAIAction TryTakeAction(EnemyAIAction aiAction)
    {
        if(enemy.TryTakeAction(aiAction.action, aiAction.targetCell)) return aiAction;

        var redo = aiAction.action.GetBestAIAction(enemy.GetGridCell());
        if (redo != null && enemy.TryTakeAction(redo.action, redo.targetCell)) return redo;

        if(aiAction.TryAlt())
        {
            var alt = aiAction.GetAltAction().GetBestAIAction(enemy.GetGridCell());
            if(alt != null && enemy.TryTakeAction(alt.action, alt.targetCell)) return alt;
            else return null;
        }
        return null;
    }
    #endregion

    #region //Choosing actions
    private EnemyAIActionList DetermineActions()
    {
        var startCell = enemy.GetGridCell();
        var lists = new List<EnemyAIActionList>();
        var validCells = moveAction.GetValidCells();
        int currentAP;

        foreach(var cell in validCells)
        {
            currentAP = enemy.GetAP();
            var list = new EnemyAIActionList();

            //Handle moving first
            if(enemy.GetGridCell() != cell)
            {
                list.AddAIAction(moveAction.GetEnemyAIAction(enemy.GetGridCell(), cell));
                currentAP -= moveAction.GetAPForMove(0);
            }

            lists.Add(MakeActionList(list, cell, currentAP));
        }

        //Sort lists from high to low total scores
        lists.Sort((x,y) => 
        {
            if(x.GetScore() > y.GetScore()) return -1;
            else if(x.GetScore() < y.GetScore()) return 1;
            return 0;
        });
        
        return lists[0];
    }

    private EnemyAIActionList MakeActionList(EnemyAIActionList list, GridCell unitCell, int currentAP)
    {
        while(currentAP > 0)
        {
            EnemyAIAction enemyAction = GetBestAction(unitCell, currentAP);
            if(enemyAction == null) break;
            list.AddAIAction(enemyAction);
            currentAP -= enemyAction.action.GetAPCost(currentAP);
        }

        if(list.HasAction(moveAction)) return list;
        if(list.GetTotalCost() + moveAction.GetAPForMove(0) >= enemy.GetAP()) return list;
        var move = moveAction.GetBestAIAction(unitCell);
        list.AddAIAction(move);
        return list;
    }

    private EnemyAIAction GetBestAction(GridCell enemyCell, int currentAP)
    {
        EnemyAIAction enemyAIAction = null;

        foreach(var action in enemy.GetActions())
        {
            //Can take action
            if(action is MoveAction) continue;
            if(!action.CanSelectAction(currentAP)) continue;
            if(currentAP < action.GetAPCost(currentAP)) continue;

            //Test action is the current best choice
            var testAction = action.GetBestAIAction(enemyCell);
            if(testAction == null) continue;
            if(testAction.score <= 0) continue;
            if(enemyAIAction != null && testAction.score > enemyAIAction.score) continue;

            //Set action
            enemyAIAction = new EnemyAIAction(testAction);
        }
        return enemyAIAction;
    }
    #endregion
}
