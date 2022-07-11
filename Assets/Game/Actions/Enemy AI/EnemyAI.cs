using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// LIMITATIONS:
/// The AI will either move first, last, or not at all. It won't act, move, and then act
/// The AI will not move multiple times in a given turn
/// Currently the AI favors movements that place it further away from enemies. Playing passively.
/// AI can't track current ammo, stock, etc when choosing an action
/// If a chosen action is no longer valid (like if attack 1 kills its target leaving no target for attack 2)
///     it will either choose a new target, try the alt action, or do nothing. It won't consider other options.

/// POSSIBLE CHANGES:
/// Make handling movement better. I first need to decide if I want there to be a limit on one or two movements per turn. Scoring moving is also weird.
///     Reason choosing between being aggressive (wanting to be near foes) or being defensive (wanting to get away from foes). I could potentially
///     make it dependent upon remaining AP, health, and a few other factors. TBD.
/// Have the AI re-evaluate its turn after each action it takes. This will have to come with a more concrete idea as to how moving will work.

/// <summary>
/// Decides and take the turn for an enemy ai
/// </summary>
[RequireComponent(typeof(Unit))]
public class EnemyAI : MonoBehaviour
{
    private Unit enemy = null;
    private bool waiting = false;


    #region //Monobehaviour
    private void Awake()
    {
        enemy = GetComponent<Unit>();
    }
    #endregion

    #region //Take turn
    public IEnumerator TakeTurn(float waitTime)
    {
        var actionList = DetermineActions();
        Func<bool> doneWaiting = () => !waiting || !enemy.IsAlive();

        foreach(var aiAction in actionList.GetAIActions())
        {
            waiting = true;
            if (enemy.TryTakeAction(aiAction.action, aiAction.targetCell))
                aiAction.PerformAction(ActionComplete);
            else
            {
                var redo = aiAction.action.GetBestAIAction(enemy.GetGridCell());
                if (redo != null && enemy.TryTakeAction(redo.action, redo.targetCell))
                    redo.PerformAction(ActionComplete);
                else if(aiAction.TryAlt())
                {
                    var alt = aiAction.GetAltAction().GetBestAIAction(enemy.GetGridCell());
                    if(alt != null && enemy.TryTakeAction(alt.action, alt.targetCell))
                        alt.PerformAction(ActionComplete);
                    else
                        continue;
                }
                else continue;
            }

            yield return new WaitUntil(doneWaiting);
            if(!enemy.IsAlive()) yield break;
            yield return new WaitForSeconds(waitTime);
        }

    }

    private void ActionComplete() => waiting = false;
    #endregion

    #region //Choosing actions
    private EnemyAIActionList DetermineActions()
    {
        var startCell = enemy.GetGridCell();
        var lists = new List<EnemyAIActionList>();
        var moveAction = enemy.GetComponent<MoveAction>();
        var validCells = moveAction.GetValidCells();

        foreach(var cell in validCells)
        {
            int currentAP = enemy.GetAP();
            var list = new EnemyAIActionList();

            //Handle moving first
            if(enemy.GetGridCell() != cell)
            {
                list.AddAIAction(moveAction.GetEnemyAIAction(enemy.GetGridCell(), cell));
                currentAP--;
            }

            //Choose actions
            while(currentAP > 0)
            {
                EnemyAIAction enemyAction = GetBestAction(cell, currentAP);
                if(enemyAction == null) break;
                list.AddAIAction(enemyAction);
                currentAP -= enemyAction.action.GetAPCost(currentAP);
            }
            lists.Add(list);
        }

        //Try a move if the enemy has not yet already
        foreach(var list in lists)
        {
            if(list.HasAction(moveAction)) continue;
            if(list.GetTotalCost() >= enemy.GetAP()) continue;
            var move = moveAction.GetBestAIAction(startCell);
            list.AddAIAction(move);
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

    private EnemyAIAction GetBestAction(GridCell enemyCell, int currentAP)
    {
        EnemyAIAction enemyAIAction = null;

        foreach(var action in enemy.GetActions())
        {
            if(action is MoveAction) continue;
            var testAction = action.GetBestAIAction(enemyCell);
            if(testAction == null) continue;
            if(!action.CanSelectAction(currentAP)) continue;
            if(currentAP < action.GetAPCost(currentAP)) continue;

            if(enemyAIAction == null || testAction.score > enemyAIAction.score) 
                enemyAIAction = new EnemyAIAction(testAction);
        }
        return enemyAIAction;
    }
    #endregion
}
