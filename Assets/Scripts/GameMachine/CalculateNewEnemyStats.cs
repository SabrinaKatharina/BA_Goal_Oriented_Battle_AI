using UnityEngine;
using System.Collections;

public class CalculateNewEnemyStats{

    private float enemyHealPointsModifier = 0.05f;
    private float enemyAttackModifier = 0.1f;
    private float enemyDefenseModifier = 0.1f;
   
    
    public enum StatType { HP, ATK, DEF};

    public int calculateStat(int baseStatValue, StatType type, int playerLevel) 
    {

        float modifier;
        if(type == StatType.HP)
        {
            modifier = enemyHealPointsModifier;
            return (baseStatValue + (int)(baseStatValue * modifier) * playerLevel);
        }
        else if (type == StatType.ATK)
        {
            modifier = enemyAttackModifier;
            return (baseStatValue + (int)(baseStatValue * modifier) * playerLevel);
        }
        else if (type == StatType.DEF)
        {
            modifier = enemyDefenseModifier;
            return (baseStatValue + (int)(baseStatValue * modifier) * playerLevel);
        }
        return 0;
    }


}
