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
    #region //Variables
    private Unit enemy = null;
    private MoveAction moveAction = null;
    [SerializeField, Range(0,10)] private int aggression = 5;
    [SerializeField, Min(1)] private int iterations = 1;
    [SerializeField] private bool iterateLists = false;
    [SerializeField] private bool iterateActions = true;
    #endregion


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
        var actionList = DetermineActions(enemy.GetGridCell());
        Func<bool> doneWaiting = () => !waiting || !enemy.IsAlive();
        int actionNo = 0;
        Debug.Log(actionList);

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
            var testList = MakeActionList(new EnemyAIActionList(actionList, actionNo), enemy.GetGridCell());
            if(!performed && testList == actionList) break;
            actionList = testList;
        }
    }

    private EnemyAIActionList DetermineActions(GridCell unitCell)
    {
        var lists = new List<EnemyAIActionList>();
        foreach(var cell in moveAction.GetValidCells(unitCell))
        {
            var list = new EnemyAIActionList(enemy.GetAP(), aggression);
            if(cell != unitCell)
            {
                var aiAction = moveAction.GetAIAction(list, unitCell, cell);
                list.AddAIAction(aiAction);
            }
            lists.Add(MakeActionList(list, cell));
        }

        //Return the best list
        lists.Sort((EnemyAIActionList a, EnemyAIActionList b) => b.GetScore() - a.GetScore());
        return lists[0];
    }
    #endregion

    #region //Get actions
    private EnemyAIActionList MakeActionList(EnemyAIActionList list, GridCell unitCell)
    {
        //Determine possible actions and locations
        var actionOptions = new Dictionary<BaseAction, List<GridCell>>();
        foreach(var action in enemy.GetActions())
        {
            if(!TryAction(list, action)) continue;
            var cells = action.GetValidCells(unitCell);
            if(cells.Count == 0) continue;
            actionOptions.Add(action, cells);
        }
        if(actionOptions.Keys.Count == 0) return null;

        //Generate lists
        var lists = new List<EnemyAIActionList>();
        int iterationCount = iterateLists ? iterations : 1;
        for(int ii = 0; ii < iterationCount; ii++)
        {
            var newList = new EnemyAIActionList(list);
            while(newList.GetAP() > 0)
            {
                EnemyAIAction enemyAction = GetBestAction(newList, unitCell, actionOptions);
                if(enemyAction == null) break;
                newList.AddAIAction(enemyAction);
            }
            lists.Add(newList);
        }

        //Return the best list
        lists.Sort((EnemyAIActionList a, EnemyAIActionList b) => b.GetScore() - a.GetScore());
        return lists[0];
    }

    private EnemyAIAction GetBestAction(EnemyAIActionList list, GridCell unitCell, Dictionary<BaseAction, List<GridCell>> actionOptions)
    {
        //Simulation set up
        int currentIteration = 0;
        var actions = new List<BaseAction>(actionOptions.Keys);
        var aiActions = new List<EnemyAIAction>();
        int iterationCount = iterateActions ? iterations : 1;

        //Make options. Weird way to break reference to the source
        var myOptions = new Dictionary<BaseAction, List<GridCell>>();
        foreach(var action in actions)
        {
            var cells = new List<GridCell>(actionOptions[action]);
            myOptions.Add(action, cells);
        }

        while(currentIteration < iterationCount)
        {
            //Check for breaking
            if(actions.Count == 0) break;

            //Get score
            var action = actions.GetRandomEntry();
            var targetCells = myOptions[action];
            if(targetCells.Count == 0) Debug.Log(action);
            var targetCell = targetCells.GetRandomEntry();
            var testAction = action.GetAIAction(list, unitCell, targetCell);

            //Remove current
            myOptions[action].Remove(targetCell);
            if(myOptions[action].Count == 0) actions.Remove(action);

            //Add AI Action
            if(testAction.score <= 0) continue;
            aiActions.Add(testAction);
            currentIteration++;
        }

        //Get Best Action
        if(aiActions.Count == 0) return null;
        aiActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.score - a.score);
        return aiActions[0];
    }

    private bool TryAction(EnemyAIActionList list, BaseAction action)
    {
        int currentAP = list.GetAP();
        if(!action.CanSelectAction(currentAP)) return false;
        if(action == moveAction)
        {
            if(currentAP < moveAction.GetAPForMove(list.ActionInListCount<MoveAction>()))
                return false;
        }
        else if(currentAP < action.GetAPCost(currentAP)) return false;
        return true;
    }
    #endregion
}