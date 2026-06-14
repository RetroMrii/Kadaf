using UnityEngine;

[System.Serializable]
public class RunHeroState
{
    public HeroData heroData;

    [Header("Health / Resources")]
    public int currentHP;
    public int currentMana;
    public int maxHP;

    [Header("Run State")]
    public bool isAvailable = true;
    public bool hasJoinedRun = true;

    [Header("XP / Leveling - Future System")]
    public int currentXP = 0;
    public int level = 1;
    public int pendingStatAllocations = 0;
}

public static class CombatState
{
    public static EnemyType[] currentEnemies;
}

public static class CombatRewards
{
    public static int lastCombatGoldReward = 0;
    public static int lastCombatXPReward = 0;

    public static int GetGoldForEnemy(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Heavy: return 18;     // Brute
            case EnemyType.Drone: return 14;
            case EnemyType.Assassin: return 11;
            case EnemyType.Warlock: return 8;    // Wizard
            case EnemyType.Grunt: return 5;
            default: return 0;
        }
    }

    public static int GetXPForEnemy(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Heavy: return 25;
            case EnemyType.Drone: return 15;
            case EnemyType.Assassin: return 25;
            case EnemyType.Warlock: return 20;
            case EnemyType.Grunt: return 15;
            default: return 0;
        }
    }

    public static int CalculateGoldReward(EnemyType[] enemies)
    {
        if (enemies == null || enemies.Length == 0)
            return 0;

        int total = 0;

        for (int i = 0; i < enemies.Length; i++)
            total += GetGoldForEnemy(enemies[i]);

        return total;
    }

    public static int CalculateXPReward(EnemyType[] enemies)
    {
        if (enemies == null || enemies.Length == 0)
            return 0;

        int total = 0;

        for (int i = 0; i < enemies.Length; i++)
            total += GetXPForEnemy(enemies[i]);

        return total;
    }
}