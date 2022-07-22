using System.Collections.Generic;
using UnityEngine;

public static class EnemyAIExtensions
{
    /// <summary>
    /// Gets scoring for aoe attacks using the basic damage scoring. Allows for modifiers if attacking the same team and only one target
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="targets"></param>
    /// <param name="vars"></param>
    /// <param name="sameTeamMult"></param>
    /// <param name="isMeMod"></param>
    /// <param name="singleTargetMult"></param>
    /// <returns></returns>
    public static int AOEScoring(this Unit attacker, IEnumerable<ITargetable> targets, AIDamageVars vars, float sameTeamMult, int isMeMod, float singleTargetMult)
    {
        float score = 0;
        int numTargets = 0;

        foreach(var targetable in targets)
        {
            float unitScore = attacker.DamageScoring(targetable, vars);

            Unit targetUnit = targetable as Unit;
            if(targetUnit != null)
            {
                if(targetUnit.IsAI()) unitScore *= sameTeamMult;
                if(targetUnit == attacker)
                {
                    if(isMeMod == 0) unitScore = 0;
                    else unitScore += isMeMod;
                }
            }
            if(unitScore > 0) numTargets++;
            score += unitScore;
        }
        if(score == 0) return 0;
        if(numTargets == 1) score *= singleTargetMult;
        return Mathf.RoundToInt(score);
    }

    /// <summary>
    /// Takes accuracy into account for basic damage scorer
    /// Attacks are always assumed to hit
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="targetable"></param>
    /// <param name="vars"></param>
    /// <param name="so"></param>
    /// <param name="attackPos"></param>
    /// <returns></returns>
    public static int AccuracyDamageScoring(this Unit attacker, bool hasSpin, ITargetable targetable, AIDamageVars vars, AccuracySO so, Vector3 attackPos)
    {
        int score = 0;
        int accuracy = so.CalculateAccuracy(attacker, attackPos, targetable);

        if(hasSpin) 
        {
            score += 50;
            var spinAction = attacker.GetAction<SpinAction>();
            accuracy += spinAction.GetAccuracyDrop();
            vars.damage *= Mathf.RoundToInt(spinAction.GetDamageBoost());
        }
        score += accuracy - 100;

        if(Random.Range(0, 101) < so.CalculateCritChance(accuracy))
            vars.damage = Mathf.RoundToInt(vars.damage * so.GetCritMult());

        score += attacker.DamageScoring(targetable, vars);
        return score;
    }

    /// <summary>
    /// Basic score obtainer that prioritizes units with low health
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="targetable"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public static int DamageScoring(this Unit attacker, ITargetable targetable, AIDamageVars vars)
    {
        if(targetable is SupplyCrate) return vars.crateMod;
        else if(!(targetable is Unit)) return vars.notUnitMod;
        else
        {
            Unit targetUnit = targetable as Unit;
            int hpLeft = Mathf.RoundToInt(targetUnit.GetHealth() - vars.damage);
            float hpDiff = targetUnit.GetTestHPPercent(hpLeft);
            float unitScore = vars.unitBase + vars.hpAdd * (1f  - hpDiff);
            if(hpDiff <= 0) unitScore += vars.killBonus;
            return Mathf.RoundToInt(unitScore);
        }
    }
}

/// <summary>
/// Parameters needed to get scoring for AI Damage attacks.
/// Score for killing the target is unitBase + hpAdd + killBonus
/// </summary>
public struct AIDamageVars
{
    public int damage; //Damage to target
    public int crateMod; //Return score if attacking a crate
    public int notUnitMod; //Return score if attacking a target that is neither unit nor crate
    public int unitBase; //Base unit value
    public int hpAdd; //Bonus for missing hp. Every hp% missing adds 1/100 of this score
    public int killBonus; //Bonus score if the attack kills

    public AIDamageVars(int damage, int unitBase, int hpAdd, int killBonus)
    {
        this.damage = damage;
        this.unitBase = unitBase;
        this.hpAdd = hpAdd;
        this.killBonus = killBonus;

        this.crateMod = 0;
        this.notUnitMod = 0;
    }

    public void SetNonUnitValues(int crateMod, int notUnitMod)
    {
        this.crateMod = crateMod;
        this.notUnitMod = notUnitMod;
    }
}