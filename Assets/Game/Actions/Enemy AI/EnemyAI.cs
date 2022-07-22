using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// LIMITATIONS:
/// AI makes one or more lists of random actions and picks the list with the highest accumulative score
/// AI can't track current ammo, stock, etc when choosing an action.
/// The AI will replan its turn after every action it takes.

/// POSSIBLE CHANGES:
/// Possibly have the AI consider the new plan rather than always take it

/// <summary>
/// Decides and take the turn for an enemy ai
/// </summary>
[RequireComponent(typeof(Unit))]
public class EnemyAI : MonoBehaviour
{
    #region //Cached Components
    private Unit enemy = null;
    private MoveAction moveAction = null;
    EnemyAIActionList myActions = new EnemyAIActionList();

    //These were made so that I could use coroutines to go through the AI and improve performance
    EnemyAIActionList bestList = new EnemyAIActionList();
    EnemyAIAction bestAction = null;
    #endregion

    #region AI behaviour
    [SerializeField, Range(0,10)] private int aggression = 5;
    [SerializeField, Min(1)] private int movementsToConsider = 5;
    [SerializeField] private bool iterateLists = false;
    [SerializeField, ShowIf("iterateLists"), Min(1)] private int listIterations = 1;
    [SerializeField] private bool iterateActions = true;
    [SerializeField, ShowIf("iterateActions"), Min(1)] private int actionIterations = 1;
    [SerializeField, Min(1), ShowIf("iterateActions")] private int actionIterationsPerFrame = 5;
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
        yield return GetStartingList(enemy.GetGridCell());
        Func<bool> doneWaiting = () => !waiting || !enemy.IsAlive();
        int actionNo = 0;

        while(enemy.GetAP() > 0)
        {
            //Reset
            bool performed = false;
            waiting = true;
            
            //Get next action. Abort if out of actions
            EnemyAIAction aiAction = myActions.GetAction(actionNo);
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
            yield return GetBestList(new EnemyAIActionList(myActions, actionNo), enemy.GetGridCell());
            if(!performed && bestList == myActions) break;
            myActions = bestList;
        }
    }

    private IEnumerator GetStartingList(GridCell unitCell)
    {
        // Get every movement option
        var lists = new List<EnemyAIActionList>();
        var dict = new Dictionary<EnemyAIActionList, GridCell>();
        foreach(var cell in moveAction.GetValidCells(unitCell))
        {
            var list = new EnemyAIActionList(enemy.GetAP(), aggression);
            if(cell != unitCell)
            {
                var aiAction = moveAction.GetAIAction(list, unitCell, cell);
                list.AddAIAction(aiAction);
                lists.Add(list);
                dict.Add(list, cell);
            }
        }

        //Choose the 5 best and add current
        lists.Sort((EnemyAIActionList a, EnemyAIActionList b) => b.GetScore() - a.GetScore());
        if(lists.Count > movementsToConsider) lists.RemoveRange(movementsToConsider, lists.Count - movementsToConsider);
        var currentPosList = new EnemyAIActionList(enemy.GetAP(), aggression);
        lists.Add(currentPosList);
        dict.Add(currentPosList, unitCell);

        //Get the best list
        for(int ii = 0; ii < lists.Count; ii++)
        {
            var list = lists[ii];
            yield return GetBestList(list, dict[list]);;
            lists[ii] = bestList;
        }
        lists.Sort((EnemyAIActionList a, EnemyAIActionList b) => b.GetScore() - a.GetScore());
        myActions = lists[0];
    }
    #endregion

    #region //Get actions
    private IEnumerator GetBestList(EnemyAIActionList list, GridCell unitCell)
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
        if(actionOptions.Keys.Count == 0)
        {
            bestList = null;
        }
        else
        {
            //Generate lists
            var lists = new List<EnemyAIActionList>();
            int iterationCount = iterateLists ? listIterations : 1;
            for(int ii = 0; ii < iterationCount; ii++)
            {
                var newList = new EnemyAIActionList(list);
                while(newList.GetAP() > 0)
                {
                    //Add action
                    yield return GetBestAction(newList, unitCell, actionOptions);
                    if(bestAction == null) break;
                    newList.AddAIAction(bestAction);

                    //Reassign position if the action moved the unit
                    if(bestAction.action == moveAction)
                        unitCell = bestAction.targetCell;
                }
                lists.Add(newList);
            }

            //Return the best list
            lists.Sort((EnemyAIActionList a, EnemyAIActionList b) => b.GetScore() - a.GetScore());
            bestList = lists[0];
        }
    }

    private IEnumerator GetBestAction(EnemyAIActionList list, GridCell unitCell, Dictionary<BaseAction, List<GridCell>> actionOptions)
    {
        //Simulation set up
        int currentIteration = 0;
        var actions = new List<BaseAction>(actionOptions.Keys);
        var aiActions = new List<EnemyAIAction>();
        int iterationCount = iterateActions ? actionIterations : 1;

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
            if(currentIteration % actionIterationsPerFrame == 0) yield return null;
        }

        //Get Best Action
        if(aiActions.Count == 0) bestAction = null;
        else
        {
            aiActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.score - a.score);
            bestAction = aiActions[0];
        }
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