using System;
using System.Collections.Generic;


/// <summary>
/// Holds a possible turn for the AI.
/// </summary>
public class EnemyAIActionList
{
    private List<EnemyAIAction> aiActions = new List<EnemyAIAction>();
    private int maxAP = 0;
    private int aggression = 5;


    #region //Constructor
    public EnemyAIActionList() { }
    
    public EnemyAIActionList(int maxAP, int aggression) 
    { 
        this.maxAP = maxAP;
        this.aggression = aggression;
    }

    /// <summary>
    /// Copy from a source list. Leave count at -1 to copy the whole list.
    /// </summary>
    /// <param name="source">The list you want to copy from</param>
    /// <param name="count">How many list items you want to copy</param>
    public EnemyAIActionList(EnemyAIActionList source, int count = -1)
    {
        this.maxAP = source.maxAP;
        this.aggression = source.aggression;
        
        if(count == -1)
        {
            aiActions = new List<EnemyAIAction>(source.aiActions);
            return;
        }

        for(int ii = 0; ii < count; ii++)
            aiActions.Add(source.aiActions[ii]);
    }
    #endregion

    #region //AI actions
    public void AddAIAction(EnemyAIAction aiAction)
    {
        aiActions.Add(aiAction);
    }

    public bool HasAction<T>() where T : BaseAction
    {
        return ActionInListCount<T>() > 0;
    }

    public int ActionInListCount<T>() where T : BaseAction
    {
        int count = 0;
        foreach(var aiAction in aiActions)
        {
            if(aiAction.action is T) count++;
        }
        return count;
    }
    #endregion

    #region //Getters
    public EnemyAIAction GetAction(int index)
    {
        if(index < 0 || index >= aiActions.Count) return null;
        else return aiActions[index];
    }
    public int GetScore()
    {
        int total = 0;
        foreach(var aiAction in aiActions)
            total += aiAction.score;

        return total;
    }

    public int GetAggression() => aggression;
    public int PassiveLevel() => 10 - aggression;
    public int GetAP()
    {
        int ap = maxAP;
        int moves = 0;
        foreach(var aiAction in aiActions)
        {
            int cost = 0;
            if(aiAction.action is MoveAction)
            {
                var moveAction = aiAction.action as MoveAction;
                cost = moveAction.GetAPForMove(moves);
                moves++;
            }
            else
                cost = aiAction.action.GetAPCost(ap);
            
            ap -= cost;
        }
        return ap;
    }
    #endregion
}