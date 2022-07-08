using System.Collections.Generic;


/// <summary>
/// Holds a possible turn for the AI.
/// </summary>
public class EnemyAIActionList
{
    private List<EnemyAIAction> aiActions = new List<EnemyAIAction>();


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
    #endregion
}