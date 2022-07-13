using System;
using System.Collections.Generic;


/// <summary>
/// Holds a possible turn for the AI.
/// </summary>
public class EnemyAIActionList : IEquatable<EnemyAIActionList>
{
    private List<EnemyAIAction> aiActions = new List<EnemyAIAction>();


    #region //Constructor
    public EnemyAIActionList() { }

    /// <summary>
    /// Copy from a source list. Leave count at -1 to copy the whole list.
    /// </summary>
    /// <param name="source">The list you want to copy from</param>
    /// <param name="count">How many list items you want to copy</param>
    public EnemyAIActionList(EnemyAIActionList source, int count = -1)
    {
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

    public bool HasAction(BaseAction action)
    {
        foreach(var aiAction in aiActions)
        {
            if(aiAction.action != action) continue;
            return true;
        }
        return false; 
    }
    #endregion

    #region //Getters
    public List<EnemyAIAction> GetAIActions() => aiActions;
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
    public int GetTotalCost()
    {
        int cost = 0;
        foreach(var aiAction in aiActions)
        {
            cost += aiAction.action.GetAPCost();
        }
        return cost;
    }

    public bool Equals(EnemyAIActionList other)
    {
        if(this.aiActions.Count != other.aiActions.Count) return false;
        for(int ii = 0; ii < aiActions.Count; ii++)
            if(this.aiActions[ii] != other.aiActions[ii]) return false;

        return true;
    }

    public override string ToString()
    {
        string output = $"Score: {GetScore()}, Actions: ";
        foreach(var aiAction in aiActions)
            output += $"{aiAction.action.GetActionName()}, ";
        
        return output.Remove(output.Length - 2);
    }
    #endregion
}